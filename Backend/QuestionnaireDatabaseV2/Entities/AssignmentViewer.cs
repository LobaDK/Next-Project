using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuestionnaireDatabaseV2.Entities;

/// <summary>
/// Represents who can view results from an assignment and what level of access they have.
/// Only supports individual user access since roles are now stored directly on User.
/// </summary>
[Table("AssignmentViewers")]
[Index(nameof(AssignmentId))]
[Index(nameof(UserId))]
public class AssignmentViewer
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid AssignmentId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets whether viewer can see individual participant responses.
    /// </summary>
    public bool CanViewResponses { get; set; } = true;

    /// <summary>
    /// Gets or sets whether viewer can see participant identities (names/emails).
    /// </summary>
    public bool CanViewIdentities { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this viewer assignment has been soft deleted.
    /// When true, viewing permissions are revoked but assignment record is preserved.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public virtual Assignment Assignment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}