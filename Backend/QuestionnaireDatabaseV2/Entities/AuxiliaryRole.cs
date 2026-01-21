using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireDatabaseV2.Entities;

[Index(nameof(Name), IsUnique = true)]
public class AuxiliaryRole
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    [Required]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Additional permissions granted by this auxiliary role on top of a user's base permissions.
    /// These are typically combined with the user's existing permissions before any removals
    /// specified in <see cref="RemovedPermissions" /> are applied.
    /// </summary>
    public UserPermissions AddedPermissions { get; set; } = UserPermissions.None;

    /// <summary>
    /// Permissions that are explicitly revoked by this auxiliary role from a user's effective
    /// permission set. When calculating effective permissions, any flags present here should be
    /// treated as removed, even if they are part of the user's base or added permissions.
    /// The exact combination logic is defined by the authorization code that consumes this entity.
    /// </summary>
    public UserPermissions RemovedPermissions { get; set; } = UserPermissions.None;

    public ICollection<User> Users { get; set; } = [];
}
