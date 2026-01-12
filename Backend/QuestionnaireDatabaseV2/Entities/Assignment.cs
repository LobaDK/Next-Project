using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireDatabaseV2.Entities;

/// <summary>
/// Represents an assignment that links a questionnaire to participants and viewers.
/// This is the core entity that controls who can participate and who can view results.
/// </summary>
[Table("Assignments")]
[Index(nameof(Title))]
[Index(nameof(Status))]
[Index(nameof(CreatedByUserId))]
[Index(nameof(QuestionnaireId))]
public class Assignment
{
    /// <summary>
    /// Gets or sets the unique identifier for this assignment.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of this assignment.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of this assignment.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the questionnaire used for this assignment.
    /// </summary>
    [Required]
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// Gets or sets whether managers can view results with anonymized participant identities.
    /// When true, managers see results without participant names or personal identifiers.
    /// </summary>
    public bool AllowAnonymizedManagerViewing { get; set; } = false;

    /// <summary>
    /// Gets or sets the current status of this assignment.
    /// </summary>
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;

    /// <summary>

    /// Gets or sets who created this assignment.
    /// </summary>
    [Required]
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets when this assignment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this assignment was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the questionnaire used for this assignment.
    /// </summary>
    public virtual Questionnaire Questionnaire { get; set; } = null!;

    /// <summary>

    /// Gets or sets the user who created this assignment.
    /// </summary>
    public virtual User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// Gets or sets the participants assigned to this assignment.
    /// </summary>
    public virtual ICollection<AssignmentParticipant> Participants { get; set; } = new List<AssignmentParticipant>();

    /// <summary>
    /// Gets or sets the responses submitted for this assignment.
    /// </summary>
    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
}