using API.DTO.Requests.Auth;
using API.DTO.Responses.Auth;
using API.Services;
using Settings.Models;

namespace API.Controllers;

/// <summary>
/// Authentication controller for handling user login and authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationBridge _authenticationBridge;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;
    private readonly LDAPSettings _ldapSettings;
    private readonly JWTSettings _jwtSettings;

    public AuthController(
        IAuthenticationBridge authenticationBridge, 
        IJwtService jwtService,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _authenticationBridge = authenticationBridge;
        _jwtService = jwtService;
        _logger = logger;
        _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
        _jwtSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
    }

    /// <summary>
    /// Authenticates a user using their credentials and generates JWT tokens
    /// </summary>
    /// <param name="userLogin">The user login credentials</param>
    /// <returns>Authentication response with JWT tokens on success</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        try
        {
            _authenticationBridge.Authenticate(userLogin.Username, userLogin.Password);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Authentication failed for user {Username}: {Message}", userLogin.Username, ex.Message);
            return Unauthorized("Invalid username or password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error for user {Username}", userLogin.Username);
            return StatusCode(StatusCodes.Status500InternalServerError, "Authentication service error");
        }

        if (_authenticationBridge.IsConnected())
        {
            try
            {
                // Get user info from authentication bridge
                BasicUserInfoWithUserID? userInfo = _authenticationBridge.SearchUser<BasicUserInfoWithUserID>(userLogin.Username);

                if (userInfo is null)
                {
                    _logger.LogWarning("User {Username} authenticated but user info not found", userLogin.Username);
                    return Unauthorized("User information not found");
                }

                // Determine user role from LDAP groups or default
                string userRole = DetermineUserRole(userInfo);

                // Create JWT user object
                JWTUser jwtUser = new()
                {
                    Guid = Guid.Parse(userInfo.UserId),
                    Username = userInfo.Username,
                    Name = userInfo.Name,
                    Role = userRole,
                    Permissions = GetUserPermissions(userRole)
                };

                // Generate tokens
                List<Claim> accessTokenClaims = _jwtService.GetAccessTokenClaims(jwtUser);
                List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(jwtUser.Guid.ToString());

                AuthenticationResponse response = new()
                {
                    AuthToken = _jwtService.GenerateAccessToken(accessTokenClaims),
                    RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
                };

                _logger.LogInformation("Successful login for user {Username}", userLogin.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing login for user {Username}", userLogin.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing login");
            }
            finally
            {
                _authenticationBridge.Dispose();
            }
        }
        
        return Unauthorized("Authentication bridge connection failed");
    }

    /// <summary>
    /// Check authentication status
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var isConnected = _authenticationBridge.IsConnected();
        return Ok(new { IsConnected = isConnected });
    }

    private string DetermineUserRole(BasicUserInfoWithUserID userInfo)
    {
        try
        {
            // Try to map LDAP role to internal role using simplified Manager/DefaultUser structure
            if (_ldapSettings?.RoleMappingsCN != null)
            {
                var roleMapping = _ldapSettings.RoleMappingsCN
                    .FirstOrDefault(x => userInfo.MemberOf?.Any(group => 
                        group.Contains(x.Value, StringComparison.OrdinalIgnoreCase)) == true);

                if (roleMapping.Key != null)
                {
                    return roleMapping.Key;
                }
            }

            // Default to DefaultUser if no specific role mapping found
            return "DefaultUser";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not determine role for user {Username}, defaulting to DefaultUser", userInfo.Username);
            return "DefaultUser";
        }
    }

    private int GetUserPermissions(string role)
    {
        // Simple permission mapping based on role
        return role.ToLower() switch
        {
            "manager" => 1, // Manager permissions
            "defaultuser" => 0, // Basic user permissions
            _ => 0 // Default to basic permissions
        };
    }
}