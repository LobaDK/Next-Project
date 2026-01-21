using API.DTO.Responses.Auth;
using QuestionnaireDatabaseV2.Entities;
using API.Services;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.Services.Authentication;

/// <summary>
/// Service interface for handling authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and handles database user creation/retrieval
    /// </summary>
    /// <param name="username">Username for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <returns>Authentication response with JWT tokens</returns>
    Task<AuthenticationResponse?> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Gets or creates a user in the database based on LDAP information
    /// </summary>
    /// <param name="userInfo">User information from LDAP</param>
    /// <param name="userRole">Determined user role</param>
    /// <returns>Database user entity</returns>
    Task<QuestionnaireDatabaseV2.Entities.User?> GetOrCreateUserAsync(BasicUserInfoWithUserID userInfo, UserRole userRole);

    /// <summary>
    /// Determines user role from LDAP group membership
    /// </summary>
    /// <param name="userInfo">User information from LDAP</param>
    /// <returns>User role string</returns>
    UserRole DetermineUserRole(BasicUserInfoWithUserID userInfo);

    /// <summary>
    /// Gets user permissions based on role
    /// </summary>
    /// <param name="role">User role</param>
    /// <returns>Permission level</returns>
    int GetUserPermissions(string role);
}