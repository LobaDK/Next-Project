using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireDatabaseV2.Entities;

/// <summary>
/// Represents a questionnaire definition that can be used to create assignments.
/// Questionnaires contain metadata and can have multiple versions for schema evolution.
/// </summary>
[Table("Questionnaires")]
[Index(nameof(Title))]
[Index(nameof(CreatedByUserId))]
public class Questionnaire
{
    /// <summary>
    /// Gets or sets the unique identifier for this questionnaire.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the questionnaire.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the questionnaire.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the current status of this questionnaire.
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public QuestionnaireStatus Status { get; set; } = QuestionnaireStatus.Draft;
 

    /// <summary>
    /// Gets or sets the user who created this questionnaire.
    /// </summary>
    [Required]
    [ForeignKey(nameof(CreatedByUser))]
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets when this questionnaire was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this questionnaire was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the JSON schema definition for this questionnaire.
    /// Contains the questions, options, validation rules, etc.
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string SchemaJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original questionnaire this was copied from.
    /// Used for tracking questionnaire lineage and versioning.
    /// </summary>
    [ForeignKey(nameof(CopiedFromQuestionnaire))]
    public Guid? CopiedFromQuestionnaireId { get; set; }

    /// <summary>
    /// Gets or sets the title of the original questionnaire this was copied from.
    /// Preserved even if original is deleted, for audit trail.
    /// </summary>
    [MaxLength(200)]
    public string? CopiedFromTitle { get; set; }

    /// <summary>
    /// Gets or sets whether this questionnaire has been soft deleted.
    /// Different from Status which tracks workflow state (Draft/Published/Archived).
    /// IsDeleted = true means soft-deleted, Status = Archived means workflow archived.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    /// <summary>    /// Gets or sets the user who created this questionnaire.
    /// </summary>
    public virtual User CreatedByUser { get; set; } = null!;

    /// <summary>    /// Gets or sets the original questionnaire this was copied from (if any).
    /// </summary>
    public virtual Questionnaire? CopiedFromQuestionnaire { get; set; }

    /// <summary>
    /// Gets or sets questionnaires that were copied from this one.
    /// </summary>
    public virtual ICollection<Questionnaire> CopiedQuestionnaires { get; set; } = new List<Questionnaire>();

    /// <summary>
    /// Gets or sets the assignments that use this questionnaire.
    /// </summary>
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    /// <summary>
    /// Gets or sets the responses submitted against this questionnaire.
    /// </summary>
    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
}