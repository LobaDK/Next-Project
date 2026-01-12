using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireDatabaseV2.Entities;

/// <summary>
/// Represents a participant in an assignment - a user who is expected to respond to the questionnaire.
/// </summary>
[Table("AssignmentParticipants")]
[Index(nameof(AssignmentId), nameof(UserId), IsUnique = true)]
public class AssignmentParticipant
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid AssignmentId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the permissions this participant has for the assignment.
    /// Determines what actions they can perform (answer, view own results, view others' results).
    /// </summary>
    public ParticipantPermissions Permissions { get; set; } = ParticipantPermissions.CanAnswerAndViewOwn;

    /// <summary>
    /// Gets or sets when this participant was added to the assignment.
    /// </summary>
    public DateTime AddedAt { get; set; }

    /// <summary>
    /// Gets or sets who added this participant (optional).
    /// </summary>
    public Guid? AddedByUserId { get; set; }

    // Navigation properties
    public virtual Assignment Assignment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual User? AddedByUser { get; set; }
}