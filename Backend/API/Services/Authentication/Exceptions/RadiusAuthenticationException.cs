namespace API.Services.Authentication.Exceptions;

public class RadiusAuthenticationException : Exception
{
    public RadiusAuthenticationErrorReasons ErrorCode { get; }
    
    public RadiusAuthenticationException(RadiusAuthenticationErrorReasons errorCode, string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public enum RadiusAuthenticationErrorReasons
{
    InvalidCredentials,
    AdditionalAuthenticationRequired,
    ServerError,
    NetworkError,
    ConfigurationError,
    TimeoutError
}