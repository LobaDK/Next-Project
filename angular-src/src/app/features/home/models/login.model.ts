export enum LoginErrorCode {
  InvalidCredentials = 'INVALID_CREDENTIALS',
  AccountDisabled    = 'ACCOUNT_DISABLED',
  AccountExpired     = 'ACCOUNT_EXPIRED',
  PasswordExpired    = 'PASSWORD_EXPIRED',
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
  IncorrectCredentials = '52e',
  AccountDisabled = '533', 
  AccountExpired = '701',
  PasswordExpired = '773'
}

export interface ADErrorResponse {
  ErrorCode: string;
  Message: string;
}

export type LoginResult = { success: true } | { success: false; code: LoginErrorCode };