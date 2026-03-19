namespace API.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication and authorization operations.
    /// Provides endpoints for user login, token refresh, logout, and user information retrieval.
    /// </summary>
    /// <remarks>
    /// This controller integrates with LDAP for user authentication and uses JWT tokens for authorization.
    /// It supports the following authentication flows:
    /// - Initial authentication via LDAP credentials
    /// - Token refresh using valid refresh tokens
    /// - User logout with token revocation
    /// - Current user information retrieval
    /// 
    /// The controller uses dependency injection for:
    /// - JWT service for token operations
    /// - Authentication bridge for LDAP integration
    /// - Unit of work pattern for data access
    /// - Configuration settings for JWT parameters
    /// - Logging for security monitoring
    /// 
    /// All endpoints include comprehensive error handling and security logging.
    /// Tokens are properly validated and refresh tokens are tracked for revocation.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user using LDAP credentials and generates JWT tokens for authorization.
        /// </summary>
        /// <param name="userLogin">The user login credentials containing username and password</param>
        /// <returns>
        /// Returns an <see cref="AuthenticationResponse"/> with access and refresh tokens on successful authentication,
        /// or an appropriate error status code on failure.
        /// </returns>
        /// <response code="200">Returns the authentication response with JWT tokens</response>
        /// <response code="401">Returned when authentication fails due to invalid credentials or user not found</response>
        /// <response code="500">Returned when there is an LDAP connection error</response>
        /// <remarks>
        /// This endpoint performs the following operations:
        /// 1. Authenticates the user against LDAP using provided credentials
        /// 2. Retrieves user information from LDAP directory
        /// 3. Maps LDAP roles to internal application roles
        /// 4. Fetches or determines user permissions from the database
        /// 5. Generates JWT access and refresh tokens
        /// 6. Returns the authentication response with tokens
        /// 
        /// The authentication process includes proper disposal of LDAP connections and comprehensive logging
        /// for security monitoring purposes.
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            return await _authService.Login(userLogin);
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh request containing the expired access token</param>
        /// <returns>
        /// Returns an <see cref="AuthenticationResponse"/> containing a new access token and refresh token if successful.
        /// Returns 401 Unauthorized if:
        /// - No authorization header is provided
        /// - The refresh token is invalid or expired
        /// - The subject claims don't match between tokens
        /// - The refresh token has been revoked
        /// Returns 500 Internal Server Error for unexpected errors.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authorization with the "RefreshToken" authentication scheme.
        /// The refresh token must be provided in the Authorization header as a Bearer token.
        /// The expired access token is passed in the request body and is used to validate the token refresh request.
        /// Upon successful validation, new access and refresh tokens are generated and returned.
        /// </remarks>
        [HttpPost("refresh")]
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            return await _authService.Refresh(request, User, Request.Headers.Authorization.ToString());
        }

        /// <summary>
        /// Logs out the user by revoking their refresh token.
        /// </summary>
        /// <returns>
        /// Returns HTTP 200 OK if the logout is successful.
        /// Returns HTTP 401 Unauthorized if the authorization header is missing or the token is invalid.
        /// Returns HTTP 500 Internal Server Error if an unexpected error occurs during the logout process.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authorization with a refresh token. The refresh token is extracted from the Authorization header,
        /// validated, and then revoked by adding it to the tracked refresh tokens list. The token is hashed using SHA-256 before storage.
        /// </remarks>
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            return await _authService.Logout(Request.Headers.Authorization.ToString());
        }

        /// <summary>
        /// Retrieves the current authenticated user's information from the access token.
        /// </summary>
        /// <returns>
        /// Returns the user information decoded from the JWT access token if authentication is successful.
        /// Returns 401 Unauthorized if the user is not authenticated or the token is invalid.
        /// Returns 403 Forbidden if the Authorization header is missing.
        /// </returns>
        /// <response code="200">Returns the current user's information from the JWT token</response>
        /// <response code="401">User is not authenticated or token is invalid</response>
        /// <response code="403">Authorization header is missing</response>
        [HttpGet("WhoAmI")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        [ProducesResponseType(typeof(JWTUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult WhoAmI()
        {
            return _authService.WhoAmI(Request.Headers.Authorization.ToString());
        }
    }

}