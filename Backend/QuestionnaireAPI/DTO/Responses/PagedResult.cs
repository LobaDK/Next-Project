using QuestionnaireAPI.DTO.Responses.User;

namespace QuestionnaireAPI.DTO.Responses
{
    /// <summary>
    /// Paginated response wrapper
    /// </summary>
    /// <typeparam name="T">The type of items in the page</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The items in this page
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage { get; set; }
    }
}