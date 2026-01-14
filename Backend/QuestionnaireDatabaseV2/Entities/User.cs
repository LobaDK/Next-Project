using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireDatabaseV2.Entities;

/// <summary>
/// Represents a user in the questionnaire system with Active Directory integration.
/// This entity stores user identity information and links to AD for authentication.
/// </summary>
[Table("Users")]
[Index(nameof(ActiveDirectoryGuid), IsUnique = true)]
[Index(nameof(UserName), IsUnique = true)]
public class User
{
    /// <summary>
    /// Gets or sets the unique database identifier for this user.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the Active Directory GUID for this user.
    /// This is the primary link to the user's identity in AD and must be unique.
    /// </summary>
    [Required]
    public Guid ActiveDirectoryGuid { get; set; }

    /// <summary>
    /// Gets or sets the username from Active Directory.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this user has been soft deleted.
    /// When true, user account is disabled but preserved for audit and data integrity.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the role of the user in the system.
    /// Managers have elevated permissions, while DefaultUser has standard access.
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public UserRole Role { get; set; } = UserRole.DefaultUser;

    /// <summary>
    /// Gets or sets when this user was first created in the system.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this user's information was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the assignment participations for this user.
    /// </summary>
    public virtual ICollection<AssignmentParticipant> AssignmentParticipants { get; set; } = new List<AssignmentParticipant>();

    /// <summary>
    /// Gets or sets the responses submitted by this user.
    /// </summary>
    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    /// <summary>
    /// Gets or sets the assignments created by this user.
    /// </summary>
    public virtual ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();

    /// <summary>
    /// Gets or sets the questionnaires created by this user.
    /// </summary>
    public virtual ICollection<Questionnaire> CreatedQuestionnaires { get; set; } = new List<Questionnaire>();
}
