namespace API.Exceptions;

/// <summary>
/// Contains authentication-related exception classes.
/// </summary>
public class AuthException
{
    /// <summary>
    /// Exception thrown when the system is in maintenance mode.
    /// </summary>
    public class MaintenanceModeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceModeException"/> class.
        /// </summary>
        public MaintenanceModeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceModeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MaintenanceModeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceModeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public MaintenanceModeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
