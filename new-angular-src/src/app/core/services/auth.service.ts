import { HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { catchError, interval, map, of, switchMap, tap, firstValueFrom, throwError, Subscription, Observable, filter } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import { ApiService } from './api.service';
import { Role, User } from '../../shared/models/user.model';
import { LoginErrorCode, LoginResult, ADErrorCode, ADErrorResponse } from '../../shared/models/login.model';

interface AuthTokens {
  authToken: string;
  refreshToken: string;
}


/**
 * Authentication service.
 *
 * Handles:
 * - Login/logout and token storage.
 * - Token refresh flow (throws if no tokens available).
 * - Online/offline server reachability with retry.
 * - Exposes auth state (`isAuthenticated$`), role (`userRole$`), and connectivity (`isOnline$`).
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {

  // Configuration - adjust these values as needed
  /**
   * How often (in ms) to retry checking server connectivity if offline.
   * Default: 5000ms = 5 seconds
   */
  private retryInterval = 5000;
  
  /**
   * How often (in ms) to check if token needs proactive refresh.
   * Default: 60000ms = 1 minute
   * Recommended: 1-2 minutes for good responsiveness
   */
  private autoRefreshCheckInterval = 60000;
  
  /**
   * How long before token expiry (in ms) to trigger proactive refresh.
   * Default: 120000ms = 2 minutes
   * Recommended: 2-5 minutes depending on token lifetime
   */
  private readonly refreshBufferMs = 2 * 60 * 1000;

  private baseUrl = environment.apiUrl;

  private tokenService = inject(TokenService);
  private apiService = inject(ApiService);

  // Private writable signals
  private _user = signal<User | null>(null);
  private _isOnline = signal<boolean>(true);

  private retrySubscription: Subscription | null = null;
  private autoRefreshSubscription: Subscription | null = null;

  // Public readonly signals
  public readonly isAuthenticated = computed(() => this._user() !== null);
  public readonly user = this._user.asReadonly();
  public readonly isOnline = this._isOnline.asReadonly();


  /**
   * Logs in with username/password.
   * - Sends `POST /auth` (form-encoded).
   * - On success: stores tokens, updates auth/role/online state.
   *
   * @returns Observable emitting API response or `false` on failure.
   */
  public login(userName: string, password: string): Observable<LoginResult> {
    const url = `${this.baseUrl}/auth`;
    const body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return this.apiService.post<AuthTokens>(url, body.toString(), undefined, headers).pipe(
      tap(({ authToken, refreshToken }) => {
        this.tokenService.setToken(authToken);
        this.tokenService.setRefreshToken(refreshToken);
        this._user.set(this.buildUserFromToken());
        this._isOnline.set(true);
        // Start automatic token refresh
        this.startAutoRefresh();
      }),
      map(() => ({ success: true } as const)),
      catchError(err => {
        const code = this.mapHttpErrorToLoginErrorCode(err);

        if (code !== LoginErrorCode.InvalidCredentials) {
          this.logout();
        }
        return of({ success: false, code });
      })
    );
  }

  /**
   * Refreshes the access token using the refresh token.
   * - Sends `POST /auth/refresh` with `{ expiredToken }` body and `Authorization: Bearer <refreshToken>`.
   * - On success: updates stored tokens.
   * - If no tokens exist: logs out and **throws**.
   *
   * @returns Observable emitting refreshed tokens or error.
   */
public refreshToken() {
  const refreshToken = this.tokenService.getRefreshToken();
  const expiredToken = this.tokenService.getToken();
  
  if (!refreshToken || !expiredToken) {
    this.logout();
    return throwError(() => new Error('No tokens to refresh'));
  }

  // Check if refresh token is expired
  if (this.tokenService.isRefreshTokenExpired()) {
    this.logout();
    return throwError(() => new Error('Refresh token expired'));
  }

  const url = `${this.baseUrl}/auth/refresh`;
  const headers = new HttpHeaders({
    'Authorization': `Bearer ${refreshToken}`,
    'Content-Type': 'application/json'
  });

  return this.apiService
    .post<{ authToken: string; refreshToken: string }>(url,   { expiredToken } , undefined, headers)
    .pipe(
      tap((res) => {
        this.tokenService.setToken(res.authToken);
        this.tokenService.setRefreshToken(res.refreshToken);
      }),
      catchError((err) => {
        this.logout();
        return throwError(() => err);
      })
    );
}

  /**
   * Logs the user out: clears tokens/state and stops offline retry loop.
   */
  public logout(): void {
    this.stopAutoRefresh();
    this.tokenService.clearToken();
    this.tokenService.clearRefreshToken();
    this.clearAuthState();
    this.stopRetrying();
  }

  /**
   * Initializes auth state on app start:
   * 1) Validates local token.
   * 2) Pings server for connectivity.
   * 3) If offline, starts periodic retries.
   *
   * @returns Promise resolving to `true` if authenticated, else `false`.
   */
  public initializeAuthState(): Promise<boolean> {
    if (!this.hasValidTokens()) {
      this.logout();
      return Promise.resolve(false);
    }

    return firstValueFrom(
      this.checkServerConnection().pipe(
        tap((serverIsOnline) => {
          if (serverIsOnline) {
            this._isOnline.set(true);
            this._user.set(this.buildUserFromToken());
            this.startAutoRefresh();
          } else {
            this._isOnline.set(false);
            this.startRetryingConnection();
          }
        })
      )
    );
  }

  /** Performs a `HEAD /system/ping` to confirm server connectivity. */
  private checkServerConnection() {
    return this.apiService.head<boolean>(`${this.baseUrl}/system/ping`).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }

  /**
   * Starts periodic connectivity checks while offline.
   * - Logs out if the token expires during offline period.
   * - Restores authenticated state once server is reachable and token is valid.
   */
  private startRetryingConnection() {
    this.stopRetrying();

    this.retrySubscription = interval(this.retryInterval)
      .pipe(
        tap(() => {
          if (!this.hasValidTokens()) {
            this.logout();
          }
        }),
        switchMap(() => this.checkServerConnection())
      )
      .subscribe((serverIsOnline) => {
        if (serverIsOnline) {
          this._isOnline.set(true);
          if (this.hasValidTokens()) {
            this._user.set(this.buildUserFromToken());
          }
          this.stopRetrying();
        } else {
          this._isOnline.set(false);
        }
      });
  }

  /** Stops any ongoing retry subscription. */
  private stopRetrying() {
    if (this.retrySubscription) {
      this.retrySubscription.unsubscribe();
      this.retrySubscription = null;
    }
  }

  /**
   * Starts automatic token refresh before expiration.
   */
  private startAutoRefresh(): void {
    this.stopAutoRefresh();
    
    this.autoRefreshSubscription = interval(this.autoRefreshCheckInterval)
      .pipe(
        filter(() => this.hasValidTokens() && this.shouldRefreshToken())
      )
      .subscribe(() => {
        this.refreshToken().subscribe({
          error: () => this.stopAutoRefresh()
        });
      });
  }

  /**
   * Stops automatic token refresh.
   */
  private stopAutoRefresh(): void {
    if (this.autoRefreshSubscription) {
      this.autoRefreshSubscription.unsubscribe();
      this.autoRefreshSubscription = null;
    }
  }

  /**
   * Checks if token should be refreshed based on expiration time.
   */
  private shouldRefreshToken(): boolean {
    const decoded = this.tokenService.getDecodedToken();
    if (!decoded?.['exp']) return false;
    
    const expiryTime = decoded['exp'] * 1000;
    const timeUntilExpiry = expiryTime - Date.now();
    
    return timeUntilExpiry <= this.refreshBufferMs;
  }

  // Helper methods
  /**
   * Checks if we have valid, non-expired tokens.
   */
  private hasValidTokens(): boolean {
    return this.tokenService.tokenExists() && !this.tokenService.isTokenExpired();
  }

  /**
   * Reads a specific claim from the decoded token.
   */
  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }
  
  /** Resets auth/role/online subjects to defaults. */
  private clearAuthState(): void {
    this._user.set(null);
    this._isOnline.set(true);
  }

/**
 * Constructs a User object from JWT token claims.
 * Extracts user information from the decoded token and validates that all required fields are present.
 * @returns A complete User object if all required claims are found and valid, otherwise null
 */
private buildUserFromToken(): User | null {
  const id = this.getTokenInfo<string>('sub');
  const userName = this.getTokenInfo<string>('unique_name');
  const fullName = this.getTokenInfo<string>('name');
  const roleStr = this.getTokenInfo<string>('role');

  const role = this.mapToRoleEnum(roleStr);
  if (id && userName && fullName && role) {
    return { id, userName, fullName, role };
  }

  return null;
}

/**
 * Maps a string value to a Role enum.
 * Compares against Role enum values (which are string enums) in a case-insensitive manner.
 * @param value - The string value from the token to map to a Role enum
 * @returns The corresponding Role enum value, or null if no match found
 */
private mapToRoleEnum(value: string | null): Role | null {
  if (!value) return null;

  switch (value.toLowerCase()) {
    case Role.Student:
      return Role.Student;
    case Role.Teacher:
      return Role.Teacher;
    case Role.Manager:
      return Role.Manager;
    default:
      return null;
  }
}

/**
 * Maps HTTP error response to login error code.
 */
private mapHttpErrorToLoginErrorCode(httpError: HttpErrorResponse): LoginErrorCode {
  // Handle 401 errors with potential AD error codes
  if (httpError.status === 401 && httpError.error) {
    try {
      const adError = this.parseADError(httpError.error);
      return this.mapADErrorCode(adError?.errorCode);
    } catch {
      return LoginErrorCode.InvalidCredentials;
    }
  }

  // HTTP status code mapping
  const statusMap: Record<number, LoginErrorCode> = {
    0: LoginErrorCode.Network,
    400: LoginErrorCode.BadRequest,
    401: LoginErrorCode.InvalidCredentials,
    403: LoginErrorCode.Forbidden,
    429: LoginErrorCode.RateLimited,
    503: LoginErrorCode.Unavailable
  };

  if (statusMap[httpError.status]) {
    return statusMap[httpError.status];
  }

  return httpError.status >= 500 && httpError.status < 600 
    ? LoginErrorCode.Server 
    : LoginErrorCode.Unknown;
}

private parseADError(error: any): ADErrorResponse | null {
  if (typeof error === 'string') {
    return JSON.parse(error);
  }
  if (error.errorCode) {
    return error;
  }
  const errorMessage = error.message;
  return errorMessage ? JSON.parse(errorMessage) : null;
}

private mapADErrorCode(errorCode?: string): LoginErrorCode {
  if (!errorCode) return LoginErrorCode.InvalidCredentials;
  
  switch (errorCode) {
    case ADErrorCode.InvalidCredentials:
      return LoginErrorCode.InvalidCredentials;
    case ADErrorCode.AccountDisabled:
      return LoginErrorCode.AccountDisabled;
    case ADErrorCode.AccountExpired:
      return LoginErrorCode.AccountExpired;
    case ADErrorCode.PasswordExpired: // Maps to 'PasswordHasExpired'
      return LoginErrorCode.PasswordExpired;
    case ADErrorCode.AccountLocked: // Maps to 'AccountIsLockedOut'
      return LoginErrorCode.AccountLocked;
    case ADErrorCode.AccountLoginError:
      return LoginErrorCode.AccountLoginError;
    default:
      return LoginErrorCode.InvalidCredentials;
  }
}

}
