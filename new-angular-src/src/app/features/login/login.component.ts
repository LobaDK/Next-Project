import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LoginErrorCode } from './login.model';
import { LanguageSwitcherComponent } from '../../core/components/language-switcher/language-switcher.component';
import { TrackCapsDirective } from '../../shared/directives/caps-lock';
import { environment } from '../../../environments/environment';

const ERROR_I18N: Record<LoginErrorCode, string> = {
  [LoginErrorCode.InvalidCredentials]: 'LOGIN.ERRORS.INVALID',
  [LoginErrorCode.AccountDisabled]:    'LOGIN.ERRORS.ACCOUNT_DISABLED',
  [LoginErrorCode.AccountExpired]:     'LOGIN.ERRORS.ACCOUNT_EXPIRED',
  [LoginErrorCode.PasswordExpired]:    'LOGIN.ERRORS.PASSWORD_EXPIRED',
  [LoginErrorCode.AccountLocked]:      'LOGIN.ERRORS.ACCOUNT_LOCKED',
  [LoginErrorCode.AccountLoginError]:  'LOGIN.ERRORS.ACCOUNT_LOGIN_ERROR',
  [LoginErrorCode.Network]:            'LOGIN.ERRORS.NETWORK',
  [LoginErrorCode.Server]:             'LOGIN.ERRORS.SERVER',
  [LoginErrorCode.Unknown]:            'LOGIN.ERRORS.GENERIC',
  [LoginErrorCode.BadRequest]:         'LOGIN.ERRORS.BAD_REQUEST',
  [LoginErrorCode.Forbidden]:          'LOGIN.ERRORS.FORBIDDEN',
  [LoginErrorCode.RateLimited]:        'LOGIN.ERRORS.RATE_LIMITED',
  [LoginErrorCode.Unavailable]:        'LOGIN.ERRORS.UNAVAILABLE',
  [LoginErrorCode.Timeout]:            'LOGIN.ERRORS.TIMEOUT',
};

@Component({
  selector: 'app-login',
  imports: [FormsModule, TranslateModule, LanguageSwitcherComponent, TrackCapsDirective],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  
  public userName = '';
  public password = '';

  errorKey: string | null = null;

  isLoading = false;
  capsLockOn = false;
  
  // Flag to control whether to show specific AD error messages or generic ones
  showSpecificErrors = environment.showSpecificErrors ?? false;

  ngOnInit() {
    // Redirect to home if already authenticated
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/home']);
    }
  }

  login() {
    if (this.isLoading) return;
    this.isLoading = true;
    this.errorKey = null;

    this.authService.login(this.userName, this.password).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe(res => {
      if (res.success) {
        this.router.navigate(['/home']);
        return;
      }
      
      // Check if we should show generic message for AD-specific errors
      if (!this.showSpecificErrors && this.isADSpecificError(res.code)) {
        this.errorKey = 'LOGIN.ERRORS.GENERIC_AD_ERROR';
      } else {
        this.errorKey = ERROR_I18N[res.code];
      }
    });
  }

  onSubmit() {
    // Prevent submission if required fields are empty
    if (!this.userName?.trim() || !this.password?.trim()) {
      this.errorKey = 'LOGIN.ERRORS.REQUIRED_FIELDS';
      return;
    }
    this.login();
  }

  onCapsLockChange(capsLockOn: boolean) {
    this.capsLockOn = capsLockOn;
  }
  
  /**
   * Checks if the error code is specific to Active Directory (not invalid credentials)
   */
  private isADSpecificError(errorCode: LoginErrorCode): boolean {
    return [
      LoginErrorCode.AccountDisabled,
      LoginErrorCode.AccountExpired,
      LoginErrorCode.PasswordExpired,
      LoginErrorCode.AccountLocked,
      LoginErrorCode.AccountLoginError
    ].includes(errorCode);
  }
}
