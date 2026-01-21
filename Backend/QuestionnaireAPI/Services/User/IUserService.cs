using QuestionnaireAPI.DTO.Responses.User;
using QuestionnaireAPI.DTO.Responses;
using QuestionnaireAPI.DTO.Requests;
using QuestionnaireDatabaseV2.Enums;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireAPI.Services.User;

/// <summary>
/// Service interface for user operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets the current authenticated user's information
    /// </summary>
    /// <param name="claimsPrincipal">The current user's claims principal</param>
    /// <returns>User information DTO or null if user not found</returns>
    Task<UserDTO?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Gets user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User information DTO</returns>
    Task<UserDTO?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Search and paginate users
    /// </summary>
    /// <param name="request">Search and pagination parameters</param>
    /// <returns>Paginated list of users</returns>
    Task<PagedResult<UserDTO>> SearchUsersAsync(UserSearchRequest request);

    /// <summary>
    /// Calculates the effective user permissions based on base permissions and auxiliary roles.
    /// </summary>
    /// <param name="basePermissions">The base permissions derived from the user's primary role.</param>
    /// <param name="auxiliaryRoles">Auxiliary roles that add or remove specific permissions.</param>
    /// <returns>The effective user permissions after applying all auxiliary roles.</returns>
    public static UserPermissions CalculateUserPermissions(UserPermissions basePermissions, List<AuxiliaryRole> auxiliaryRoles)
    {
        if (auxiliaryRoles is null || auxiliaryRoles.Count == 0)
        {
            return basePermissions;
        }

        UserPermissions enabled = UserPermissions.None;
        UserPermissions disabled = UserPermissions.None;

        foreach (var auxiliaryRole in auxiliaryRoles)
        {
            enabled |= auxiliaryRole.AddedPermissions;
            disabled |= auxiliaryRole.RemovedPermissions;
        }

        basePermissions |= enabled;
        basePermissions &= ~disabled;

        return basePermissions;
    }
    
    public static UserPermissions ConvertRoleToUserPermissionsAsync(UserRole role)
    {
        return role switch
        {
            UserRole.Manager => UserPermissions.Management,
            UserRole.Student => UserPermissions.Student,
            UserRole.Teacher => UserPermissions.Teacher,
            UserRole.DefaultUser => UserPermissions.None,
            _ => UserPermissions.None,
        };
    }

}