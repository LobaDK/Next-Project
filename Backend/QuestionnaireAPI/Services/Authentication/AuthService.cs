using API.DTO.Responses.Auth;
using API.Services;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;
using QuestionnaireDatabaseV2.Enums;
using Settings.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QuestionnaireAPI.Services.User;
using QuestionnaireAPI.Interfaces;
using QuestionnaireAPI.Exceptions;
using System.Security.Cryptography;

namespace QuestionnaireAPI.Services.Authentication;

/// <summary>
/// Service for handling authentication operations and user management
/// </summary>
public class AuthService(
    IAuthenticationBridge authenticationBridge,
    IJwtService jwtService,
    ILogger<AuthService> logger,
    IConfiguration configuration,
    QuestionnaireDbContext dbContext,
    IMaintenanceMonitor maintenanceMonitor) : IAuthService
{
    private readonly IAuthenticationBridge _authenticationBridge = authenticationBridge;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly LDAPSettings _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
    private readonly SystemSettings _systemSettings = ConfigurationBinderService.Bind<SystemSettings>(configuration);
    private readonly QuestionnaireDbContext _dbContext = dbContext;
    private readonly IMaintenanceMonitor _maintenanceMonitor = maintenanceMonitor;

    public async Task<AuthenticationResponse?> AuthenticateAsync(string username, string password)
    {
        // Check maintenance mode FIRST
        if (_maintenanceMonitor.IsMaintenanceEnabled)
        {
            return AuthenticateMaintenanceMode(username, password);
        }

        try
        {
            // Authenticate with LDAP/AD
            _authenticationBridge.Authenticate(username, password);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Authentication failed for user {Username}: {Message}", username, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error for user {Username}", username);
            throw;
        }

        if (!_authenticationBridge.IsConnected())
        {
            throw new InvalidOperationException("Authentication bridge connection failed");
        }

        try
        {
            // Get user info from authentication bridge
            BasicUserInfoWithUserID? userInfo = _authenticationBridge.SearchUser<BasicUserInfoWithUserID>(username);

            if (userInfo is null)
            {
                _logger.LogWarning("User {Username} authenticated but user info not found", username);
                return null;
            }

            // Determine user role from LDAP groups or default
            UserRole userRole = DetermineUserRole(userInfo);

            // Check if user exists in database, if not create it
            var dbUser = await GetOrCreateUserAsync(userInfo, userRole);
            if (dbUser == null)
            {
                _logger.LogError("Failed to get or create user {Username} in database", username);
                return null;
            }

            // Create JWT user object
            JWTUser jwtUser = new()
            {
                Guid = dbUser.Id, // Use database user ID
                Username = userInfo.Username,
                Name = userInfo.Name,
                Role = userRole.ToString(),
                Permissions = GetUserPermissions(userRole.ToString())
            };

            // Generate tokens
            List<Claim> accessTokenClaims = _jwtService.GetAccessTokenClaims(jwtUser);
            List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(jwtUser.Guid.ToString());

            AuthenticationResponse response = new()
            {
                AuthToken = _jwtService.GenerateAccessToken(accessTokenClaims),
                RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
            };

            _logger.LogInformation("Successful login for user {Username}", username);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing login for user {Username}", username);
            throw;
        }
        finally
        {
            _authenticationBridge.Dispose();
        }
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
            Role = UserRole.Admin.ToString(),
            Permissions = GetUserPermissions(UserRole.Admin.ToString())
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

    public async Task<QuestionnaireDatabaseV2.Entities.User?> GetOrCreateUserAsync(BasicUserInfoWithUserID userInfo, UserRole userRole)
    {
        try
        {
            var activeDirectoryGuid = Guid.Parse(userInfo.UserId);
            
            // First, try to get existing user by Active Directory GUID
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.ActiveDirectoryGuid == activeDirectoryGuid && !u.IsDeleted);

            if (existingUser != null)
            {
                // User exists, just return it (no updates needed)
                _logger.LogInformation("Found existing user {Username} in database", userInfo.Username);
                return existingUser;
            }

            // User doesn't exist, create new user
            var newUser = new QuestionnaireDatabaseV2.Entities.User
            {
                Id = Guid.NewGuid(),
                ActiveDirectoryGuid = activeDirectoryGuid,
                UserName = userInfo.Username,
                FullName = userInfo.Name,
                Role = userRole,
                Permissions = IUserService.ConvertRoleToUserPermissionsAsync(userRole),
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created new user {Username} in database with ID {UserId}", userInfo.Username, newUser.Id);
            return newUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting or creating user {Username} in database", userInfo.Username);
            return null;
        }
    }

    public UserRole DetermineUserRole(BasicUserInfoWithUserID userInfo)
    {
        try
        {
            // Check if user is in the Manager group
            if (!string.IsNullOrEmpty(_ldapSettings.ManagerGroupCN))
            {
                bool isManager = userInfo.MemberOf?.Any(group => 
                    group.Contains(_ldapSettings.ManagerGroupCN, StringComparison.OrdinalIgnoreCase)) == true;

                if (isManager)
                {
                    return UserRole.Manager;
                }
            }

            if (!string.IsNullOrEmpty(_systemSettings.AdminUsername))
            {
                // Check for admin user
                if (string.Equals(userInfo.Username, _systemSettings.AdminUsername, StringComparison.OrdinalIgnoreCase))
                {
                    return UserRole.Admin;
                }
            }

            if (_ldapSettings.RoleMappingsCN.Count > 0)
            {
                foreach (var (role, groupCN) in _ldapSettings.RoleMappingsCN)
                {
                    if (userInfo.MemberOf?.Any(group => 
                        group.Contains(groupCN, StringComparison.OrdinalIgnoreCase)) == true)
                    {
                        return Enum.Parse<UserRole>(role);
                    }
                }
            }

            // Default to DefaultUser if not in Manager group
            return UserRole.DefaultUser;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not determine role for user {Username}, defaulting to DefaultUser", userInfo.Username);
            return UserRole.DefaultUser;
        }
    }

    public int GetUserPermissions(string role)
    {
        return role switch
        {
            nameof(UserRole.Admin) => (int)UserPermissions.Admin,
            nameof(UserRole.Manager) => (int)UserPermissions.Management,
            nameof(UserRole.Student) => (int)UserPermissions.Student,
            nameof(UserRole.Teacher) => (int)UserPermissions.Teacher,
            nameof(UserRole.DefaultUser) => (int)UserPermissions.None,
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