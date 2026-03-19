namespace API.Services;

using ILogger = Microsoft.Extensions.Logging.ILogger;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IAuthenticationBridge _authenticationBridge;
    private readonly LDAPSettings _ldapSettings;
    private readonly SystemSettings _systemSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IMaintenanceMonitor _maintenanceMonitor;

    public AuthService(
        IJwtService jwtService,
        IAuthenticationBridge ldapService,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILoggerFactory loggerFactory,
        IMaintenanceMonitor maintenanceMonitor)
    {
        _jwtService = jwtService;
        _authenticationBridge = ldapService;
        _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
        _systemSettings = ConfigurationBinderService.Bind<SystemSettings>(configuration);
        _unitOfWork = unitOfWork;
        _logger = loggerFactory.CreateLogger(GetType());
        _maintenanceMonitor = maintenanceMonitor;
    }

    public async Task<IActionResult> Login(UserLogin userLogin)
    {
        try
        {
            if (_maintenanceMonitor.IsMaintenanceEnabled)
            {
                return new OkObjectResult(AuthenticateMaintenanceMode(userLogin.Username, userLogin.Password));
            }
            else
            {
                _authenticationBridge.Authenticate(userLogin.Username, userLogin.Password);
            }
        }
        catch (ConnectionError)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (UnauthorizedAccessException e)
        {
            return new UnauthorizedObjectResult(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return new ObjectResult(e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
        catch (LdapAuthenticationErrorException e)
        {
            return new UnauthorizedObjectResult(new AuthenticationError()
            {
                ErrorCode = e.ErrorCode.ToString(),
                Message = e.Message
            });
        }

        if (!_authenticationBridge.IsConnected())
        {
            return new UnauthorizedResult();
        }

        BasicUserInfoWithUserID? ldapUser = _authenticationBridge.SearchUser<BasicUserInfoWithUserID>(userLogin.Username);

        if (ldapUser is null)
        {
            _authenticationBridge.Dispose();
            _logger.LogWarning(UserLogEvents.UserLogIn, "User {username} successfully logged in, yet the user query returned nothing.", userLogin.Username);
            return new UnauthorizedResult();
        }

        Guid userGuid = new(ldapUser.UserId);

        string userRole;
        try
        {
            userRole = _ldapSettings.RoleMappingsCN.First(x => ldapUser.MemberOf.Any(y => y.Contains(x.Value, StringComparison.OrdinalIgnoreCase))).Key;
        }
        catch (Exception e)
        {
            _authenticationBridge.Dispose();
            _logger.LogWarning(UserLogEvents.UserLogIn, e, "User {username} successfully logged in, yet the user role could not be determined. {Message}", userLogin.Username, e.Message);
            return new UnauthorizedResult();
        }

        FullUser? user = await _unitOfWork.User.GetUserAsync(userGuid);

        UserPermissions permissions = user is not null ? user.Permissions : (UserPermissions)Enum.Parse(typeof(UserPermissions), userRole, true);
        JWTUser jwtUser = new()
        {
            Guid = userGuid,
            Username = ldapUser.Username,
            Name = ldapUser.Name,
            Role = userRole,
            Permissions = (int)permissions
        };

        _authenticationBridge.Dispose();

        List<Claim> accessTokenClaims = _jwtService.GetAccessTokenClaims(jwtUser);
        List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(jwtUser.Guid.ToString());

        AuthenticationResponse response = new()
        {
            AuthToken = _jwtService.GenerateAccessToken(accessTokenClaims),
            RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
        };

        _logger.LogInformation(UserLogEvents.UserLogIn, "Successfull login from {username}.", userLogin.Username);

        return new OkObjectResult(response);
    }

    public async Task<IActionResult> Refresh(RefreshRequest request, ClaimsPrincipal user, string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return new UnauthorizedResult();
        }

        string token = authorizationHeader.Split(' ').Last();

        if (!_jwtService.TokenIsValid(token, _jwtService.GetRefreshTokenValidationParameters()))
        {
            return new UnauthorizedResult();
        }

        ClaimsPrincipal principal = _jwtService.GetPrincipalFromExpiredToken(request.ExpiredToken);

        if (user.FindFirstValue(JwtRegisteredClaimNames.Sub) != principal.FindFirstValue(JwtRegisteredClaimNames.Sub))
        {
            return new UnauthorizedResult();
        }

        byte[] hashedToken = Crypto.ToSha256(token);
        if (principal is null || await _unitOfWork.TrackedRefreshToken.IsTokenRevoked(hashedToken))
        {
            return new UnauthorizedResult();
        }

        List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        AuthenticationResponse response = new()
        {
            AuthToken = _jwtService.GenerateAccessToken(principal.Claims),
            RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
        };

        return new OkObjectResult(response);
    }

    public async Task<IActionResult> Logout(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return new UnauthorizedResult();
        }

        string token = authorizationHeader.Split(' ').Last();

        if (!_jwtService.TokenIsValid(token, _jwtService.GetRefreshTokenValidationParameters()))
        {
            return new UnauthorizedResult();
        }

        byte[] encryptedToken = Crypto.ToSha256(token);

        await _unitOfWork.TrackedRefreshToken.RevokeToken(encryptedToken);
        await _unitOfWork.SaveChangesAsync();

        return new OkResult();
    }

    public IActionResult WhoAmI(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return new ForbidResult();
        }

        string token = authorizationHeader.Replace("Bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
        return new OkObjectResult(_jwtService.DecodeAccessToken(token));
    }

    private AuthenticationResponse? AuthenticateMaintenanceMode(string username, string password)
    {
        // Only allow admin user during maintenance
        if (!IsAdminUser(username))
        {
            _logger.LogWarning("Login attempt during maintenance mode for non-admin user {Username}", username);
            throw new AuthException.MaintenanceModeException("The server is currently in maintenance mode. Only administrators can log in.");
        }

        // Validate password against internal admin credentials (skip LDAP/database)
        if (string.IsNullOrEmpty(_systemSettings.AdminPassword) || !string.Equals(password, _systemSettings.AdminPassword))
        {
            _logger.LogWarning("Authentication failed for admin user {Username} during maintenance mode", username);
            throw new UnauthorizedAccessException("Invalid admin credentials");
        }

        _logger.LogInformation("Admin user {Username} successfully authenticated during maintenance mode (LDAP/DB bypassed)", username);

        // Create JWT user object without database lookup
        JWTUser jwtUser = new()
        {
            Guid = Guid.Empty, // Placeholder GUID for maintenance mode
            Username = username,
            Name = _systemSettings.AdminUsername ?? "Administrator",
            Role = UserRoles.Admin.ToString(),
            Permissions = GetUserPermissions(UserRoles.Admin.ToString())
        };

        // Generate tokens
        List<Claim> accessTokenClaims = _jwtService.GetAccessTokenClaims(jwtUser);
        List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(Guid.NewGuid().ToString()); // Use temp GUID for refresh token

        AuthenticationResponse response = new()
        {
            AuthToken = _jwtService.GenerateAccessToken(accessTokenClaims),
            RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
        };

        return response;
    }

    public int GetUserPermissions(string role)
    {
        return role switch
        {
            nameof(UserRoles.Admin) => (int)UserPermissions.Admin,
            nameof(UserRoles.Student) => (int)UserPermissions.Student,
            nameof(UserRoles.Teacher) => (int)UserPermissions.Teacher,
            nameof(UserRoles.DefaultUser) => (int)UserPermissions.None,
            _ => (int)UserPermissions.None,
        };
    }

    private bool IsAdminUser(string username)
    {
        // Check against configured admin username
        return !string.IsNullOrEmpty(_systemSettings.AdminUsername) &&
               string.Equals(username, _systemSettings.AdminUsername, StringComparison.OrdinalIgnoreCase);
    }
}
