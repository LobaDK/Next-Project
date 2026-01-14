using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2;
using QuestionnaireAPI.DTO.Responses.User;
using QuestionnaireAPI.DTO.Responses;
using QuestionnaireAPI.DTO.Requests;
using QuestionnaireAPI.Mappers;
using System.Security.Claims;

namespace QuestionnaireAPI.Services.User;

/// <summary>
/// Service for user operations
/// </summary>
public class UserService : IUserService
{
    private readonly QuestionnaireDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    // Constants for pagination limits
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int MinPage = 1;

    public UserService(QuestionnaireDbContext dbContext, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UserDTO?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userId = ExtractUserIdFromClaims(claimsPrincipal);
            if (userId == null)
            {
                return null;
            }

            return await GetUserByIdAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user from claims");
            return null;
        }
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .Select(UserMapper.ToUserDtoExpression)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found or is deleted", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
            return null;
        }
    }

    public async Task<PagedResult<UserDTO>> SearchUsersAsync(UserSearchRequest request)
    {
        try
        {
            _logger.LogInformation("Searching users with criteria: SearchTerm='{SearchTerm}', Role={Role}, Page={Page}, PageSize={PageSize}", 
                request.SearchTerm, request.Role, request.Page, request.PageSize);

            ValidateAndSanitizeRequest(request);
            var query = BuildUserSearchQuery(request);
            
            var totalItems = await query.CountAsync();
            var users = await ExecutePaginatedQuery(query, request);
            
            return CreatePagedResult(users, request, totalItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with search term '{SearchTerm}' and role '{Role}'", request.SearchTerm, request.Role);
            return CreateEmptyPagedResult(request);
        }
    }

    /// <summary>
    /// Extracts user ID from JWT token claims
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal containing JWT claims</param>
    /// <returns>User ID if found and valid, null otherwise</returns>
    private Guid? ExtractUserIdFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.FindFirst("sub") ?? claimsPrincipal.FindFirst("user_id");
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("User ID claim not found or invalid in token");
            return null;
        }

        return userId;
    }

    /// <summary>
    /// Validates and sanitizes the user search request
    /// </summary>
    /// <param name="request">The request to validate</param>
    private static void ValidateAndSanitizeRequest(UserSearchRequest request)
    {
        if (request.PageSize > MaxPageSize)
        {
            request.PageSize = MaxPageSize;
        }
        if (request.PageSize < 1)
        {
            request.PageSize = DefaultPageSize;
        }
        if (request.Page < MinPage)
        {
            request.Page = MinPage;
        }
    }

    /// <summary>
    /// Builds the user search query based on the request criteria
    /// </summary>
    /// <param name="request">The search request</param>
    /// <returns>Queryable for users matching the criteria</returns>
    private IQueryable<QuestionnaireDatabaseV2.Entities.User> BuildUserSearchQuery(UserSearchRequest request)
    {
        var query = _dbContext.Users
            .Where(u => !u.IsDeleted);

        // Apply search filter using case-insensitive comparison
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(u => 
                u.UserName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.FullName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Apply role filter
        if (request.Role.HasValue)
        {
            query = query.Where(u => u.Role == request.Role.Value);
        }

        return query;
    }

    /// <summary>
    /// Executes the paginated query and returns the mapped DTOs
    /// </summary>
    /// <param name="query">The query to execute</param>
    /// <param name="request">The pagination request</param>
    /// <returns>List of user DTOs</returns>
    private async Task<List<UserDTO>> ExecutePaginatedQuery(IQueryable<QuestionnaireDatabaseV2.Entities.User> query, UserSearchRequest request)
    {
        return await query
            .OrderBy(u => u.FullName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(UserMapper.ToUserDtoExpression)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a paged result from the users and request parameters
    /// </summary>
    /// <param name="users">The users to include in the result</param>
    /// <param name="request">The original request</param>
    /// <param name="totalItems">Total number of items matching the criteria</param>
    /// <returns>Paged result</returns>
    private static PagedResult<UserDTO> CreatePagedResult(List<UserDTO> users, UserSearchRequest request, int totalItems)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);
        
        return new PagedResult<UserDTO>
        {
            Items = users,
            CurrentPage = request.Page,
            TotalPages = totalPages,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            HasNextPage = request.Page < totalPages,
            HasPreviousPage = request.Page > 1
        };
    }

    /// <summary>
    /// Creates an empty paged result for error scenarios
    /// </summary>
    /// <param name="request">The original request</param>
    /// <returns>Empty paged result</returns>
    private static PagedResult<UserDTO> CreateEmptyPagedResult(UserSearchRequest request)
    {
        return new PagedResult<UserDTO>
        {
            Items = new List<UserDTO>(),
            CurrentPage = request.Page,
            TotalPages = 0,
            PageSize = request.PageSize,
            TotalItems = 0,
            HasNextPage = false,
            HasPreviousPage = false
        };
    }
}