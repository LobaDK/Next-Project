export enum LoginErrorCode {
  InvalidCredentials = 'INVALID_CREDENTIALS',
  AccountDisabled    = 'ACCOUNT_DISABLED',
  AccountExpired     = 'ACCOUNT_EXPIRED',
  PasswordExpired    = 'PASSWORD_EXPIRED',
  AccountLocked      = 'ACCOUNT_LOCKED',
  AccountLoginError  = 'ACCOUNT_LOGIN_ERROR',
  Network            = 'NETWORK',
  Forbidden          = 'FORBIDDEN',
  RateLimited        = 'RATE_LIMITED',
  BadRequest         = 'BAD_REQUEST',
  Server             = 'SERVER',
  Unavailable        = 'UNAVAILABLE',
  Timeout            = 'TIMEOUT',
  Unknown            = 'UNKNOWN',
}

export enum ADErrorCode {
  InvalidCredentials = 'InvalidCredentials',
  AccountDisabled = 'AccountDisabled', 
  AccountExpired = 'AccountExpired',
  PasswordExpired = 'PasswordHasExpired',
  AccountLocked = 'AccountIsLockedOut',
  AccountLoginError = 'AccountLoginError'
}

export interface ADErrorResponse {
  errorCode: string;
  message: string;
}

export type LoginResult = { success: true } | { success: false; code: LoginErrorCode };

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
