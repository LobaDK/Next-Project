using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.DTO.Responses.User;

/// <summary>
/// DTO for user information responses
/// </summary>
public class UserDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for this user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role of the user in the system.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets when this user was first created in the system.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}