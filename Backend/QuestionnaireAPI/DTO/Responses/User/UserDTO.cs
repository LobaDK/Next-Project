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
    public required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public required string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public required string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role of the user in the system.
    /// </summary>
    public required UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets the permissions assigned to the user.
    /// </summary>
    [JsonIgnore]
    public required UserPermissions Permissions { get; set; }

    /// <summary>
    /// Gets the integer value of the user's permissions for API responses.
    /// </summary>
    [JsonPropertyName("permissions")]
    public int PermissionsValue => (int)Permissions;

    /// <summary>
    /// Gets or sets when this user was first created in the system.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}