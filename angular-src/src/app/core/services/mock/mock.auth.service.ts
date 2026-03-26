import { Injectable, inject, signal, computed } from '@angular/core';
import { BehaviorSubject, Observable, of, interval, throwError } from 'rxjs';
import { TokenService } from '../token.service';
import { User, Role } from '../../../shared/models/user.model';
import { LoginResult, LoginErrorCode } from '../../../features/home/models/login.model';
import { IAuthService } from '../../interfaces/service.interfaces';
import { QuestionnaireSessionService } from '../questionnaire-session.service';

/**
 * A mock AuthService to use in tests or dev without hitting real endpoints.
 * Provides the same interface & public methods as the real AuthService.
 */
@Injectable({
  providedIn: 'root',
})
export class MockAuthService implements IAuthService {
  private tokenService = inject(TokenService);
  private questionnaireSessionService = inject(QuestionnaireSessionService);

  // Private writable signals
  private _user = signal<User | null>(null);
  private _isOnline = signal<boolean>(true);

  // Public readonly signals  
  public readonly isAuthenticated = computed(() => this._user() !== null);
  public readonly user = this._user.asReadonly();
  public readonly isOnline = this._isOnline.asReadonly();

  private readonly TEST_USERS = [
    {
      userName: 'adminUser',
      password: 'password',
      id: 'mockAdminId',
      role: Role.Admin,
      fullName: 'Admin User'
    },
    {
      userName: 'teacherUser',
      password: 'password',
      id: 'mockId12345',
      role: Role.Teacher,
      fullName: 'Teacher User'
    },
    {
      userName: 'studentUser',
      password: 'password',
      id: 'mockStudentId',
      role: Role.Student,
      fullName: 'Student User'
    },
    {
      userName: 'resultsViewerUser',
      password: 'password',
      id: 'mockResultsViewerId',
      role: Role.ResultsViewer,
      fullName: 'Results Viewer User'
    },
    {
      userName: 'extendedAdminUser',
      password: 'password',
      id: 'mockExtendedAdminId',
      role: Role.ExtendedAdmin,
      fullName: 'Extended Admin User'
    },
  ];


  constructor() {
    // Initialize state but don't await since constructor can't be async
    this.initializeAuthState();
  }


  public login(userName: string, password: string): Observable<LoginResult> {
    // Find a matching user in our test "database"
    const foundUser = this.TEST_USERS.find(
      (u) => u.userName === userName && u.password === password
    );

    if (foundUser) {
      // Create a fake token that includes the userId & role as the JWT payload
      const fakeToken = this.createFakeToken(foundUser.id, foundUser.role, 3600);
      this.tokenService.setToken(fakeToken);
      
      // Create fake refresh token
      const fakeRefreshToken = this.createFakeToken(foundUser.id, foundUser.role, 7200);
      this.tokenService.setRefreshToken(fakeRefreshToken);

      // Update local auth state with User object
      const user: User = {
        id: foundUser.id,
        userName: foundUser.userName,
        fullName: foundUser.fullName,
        role: foundUser.role
      };

      this._user.set(user);
      this.questionnaireSessionService.clearSessionsForOtherUsers(user.id);
      this._isOnline.set(true);

      return of({ success: true } as const);
    } else {
      this.clearAuthState();
      return of({ success: false, code: LoginErrorCode.InvalidCredentials });
    }
  }

  public logout(): void {
    this.questionnaireSessionService.clearAllSessions();
    this.tokenService.clearToken();
    this.tokenService.clearRefreshToken();
    this.clearAuthState();
  }

  public refreshToken(): Observable<any> {
    const refreshToken = this.tokenService.getRefreshToken();
    const expiredToken = this.tokenService.getToken();
    if (!refreshToken || !expiredToken) {
      this.logout();
      return throwError(() => new Error('No tokens to refresh'));
    }

    // Simulate token refresh - create new tokens
    const user = this._user();
    if (user) {
      const newToken = this.createFakeToken(user.id, user.role, 3600);
      const newRefreshToken = this.createFakeToken(user.id, user.role, 7200);

      this.tokenService.setToken(newToken);
      this.tokenService.setRefreshToken(newRefreshToken);

      return of({ authToken: newToken, refreshToken: newRefreshToken });
    }

    return throwError(() => new Error('No user found for refresh'));
  }

  public getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  public getUserRole(): string | null {
    return this.getTokenInfo<string>('role');
  }

  /**
   * Initialize auth state on app start - matches real AuthService signature.
   */
  public initializeAuthState(): Promise<boolean> {
    const tokenExists = this.tokenService.tokenExists();
    const tokenValid = !this.tokenService.isTokenExpired();

    if (tokenExists && tokenValid) {
      this._isOnline.set(true);
      this._user.set(this.buildUserFromToken());
      return Promise.resolve(true);
    } else {
      this.clearAuthState();
      return Promise.resolve(false);
    }
  }

  /**
   * Utility to create a fake JWT so TokenService can decode it.
   */
  private createFakeToken(id: string, userRole: Role, expiresInSeconds: number): string {
    const testUser = this.TEST_USERS.find(u => u.id === id);
    const header = { alg: 'HS256', typ: 'JWT' };
    const payload = {
      sub: id,
      unique_name: testUser?.userName || 'mockuser',
      name: testUser?.fullName || 'Mock User',
      role: userRole,
      exp: Math.floor(Date.now() / 1000) + expiresInSeconds,
    };
    const base64UrlEncode = (obj: any) =>
      btoa(JSON.stringify(obj))
        .replace(/=/g, '')
        .replace(/\+/g, '-')
        .replace(/\//g, '_');

    return `${base64UrlEncode(header)}.${base64UrlEncode(payload)}.signature-placeholder`;
  }

  /**
   * Builds User object from token claims - matches real AuthService.
   */
  private buildUserFromToken(): User | null {
    const id = this.getTokenInfo<string>('sub');
    const userName = this.getTokenInfo<string>('unique_name');
    const fullName = this.getTokenInfo<string>('name');
    const roleStr = this.getTokenInfo<string>('role');

    // Map role string to enum (like real AuthService)
    const role = this.mapToRoleEnum(roleStr);
    if (id && userName && fullName && role) {
      return { id, userName, fullName, role };
    }

    return null;
  }

  /**
   * Maps a string value to a Role enum - matches real AuthService.
   */
  private mapToRoleEnum(value: string | null): Role | null {
    if (!value) return null;

    switch (value.toLowerCase()) {
      case Role.Student:
        return Role.Student;
      case Role.Teacher:
        return Role.Teacher;
      case Role.Admin:
        return Role.Admin;
      case Role.ResultsViewer:
        return Role.ResultsViewer;
      case Role.ExtendedAdmin:
        return Role.ExtendedAdmin;
      default:
        return null;
    }
  }

  /**
   * Parse token to get a specific key from payload
   */
  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }

  /**
   * Clear all local auth-related state
   */
  private clearAuthState(): void {
    this._user.set(null);
    this._isOnline.set(true);
  }
}
