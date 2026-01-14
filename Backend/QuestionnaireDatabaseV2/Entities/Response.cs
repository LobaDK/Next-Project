using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuestionnaireDatabaseV2.Entities;

/// <summary>
/// Represents a participant's response to an assignment's questionnaire.
/// Stores answers as JSON and maintains links to the assignment and questionnaire version.
/// </summary>
[Table("Responses")]
[Index(nameof(AssignmentId))]
[Index(nameof(ParticipantId))]
[Index(nameof(SubmittedAt))]
[Index(nameof(AssignmentId), nameof(ParticipantId))]
public class Response
{
    /// <summary>
    /// Gets or sets the unique identifier for this response.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the assignment this response belongs to.
    /// </summary>
    [Required]
    [ForeignKey(nameof(Assignment))]
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Gets or sets the user who submitted this response.
    /// </summary>
    [Required]
    [ForeignKey(nameof(Participant))]
    public Guid ParticipantId { get; set; }

    /// <summary>
    /// Gets or sets the questionnaire this response was submitted against.
    /// Maintains integrity by storing questionnaire ID at time of response.
    /// </summary>
    [Required]
    [ForeignKey(nameof(Questionnaire))]
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// Gets or sets the participant's answers as JSON.
    /// Structure should match the questionnaire's schema.
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string AnswersJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this response is considered complete.
    /// Allows for partial/draft responses to be saved.
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Gets or sets when this response was first created (for drafts).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this response was submitted/completed.
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the assignment this response belongs to.
    /// </summary>
    public virtual Assignment Assignment { get; set; } = null!;

    /// <summary>
    /// Gets or sets the participant who submitted this response.
    /// </summary>
    public virtual User Participant { get; set; } = null!;

    /// <summary>
    /// Gets or sets the questionnaire this response was submitted against.
    /// </summary>
    public virtual Questionnaire Questionnaire { get; set; } = null!;
}