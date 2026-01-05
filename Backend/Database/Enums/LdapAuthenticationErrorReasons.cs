namespace Database.Enums;

public enum LdapAuthenticationErrorReasons
{
    InvalidCredentials,
    AccountDisabled,
    AccountExpired,
    AccountIsLockedOut,
    PasswordHasExpired,
    AccountLoginError
}
