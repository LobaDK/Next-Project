using QuestionnaireAPI.DTO.Responses.User;
using QuestionnaireAPI.DTO.Responses;
using QuestionnaireAPI.DTO.Requests;
using System.Security.Claims;

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
}