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