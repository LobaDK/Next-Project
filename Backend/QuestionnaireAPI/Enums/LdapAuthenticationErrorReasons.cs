namespace API.Enums;

public enum LdapAuthenticationErrorReasons
{
    InvalidCredentials,
    AccountDisabled,
    AccountExpired,
    AccountIsLockedOut,
    PasswordHasExpired,
    AccountLoginError
}
