using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.DTO.Requests
{
    /// <summary>
    /// Request parameters for searching and paginating users
    /// </summary>
    public class UserSearchRequest
    {
        /// <summary>
        /// Search term to filter by name (searches both UserName and FullName)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filter by user role
        /// </summary>
        public UserRole? Role { get; set; }

        /// <summary>
        /// Page number (1-based, default: 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default: 20, max: 100)
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}