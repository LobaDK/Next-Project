using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireDatabaseV2;

/// <summary>
/// Extensions and helper methods partial class for QuestionnaireDbContext.
/// Contains SaveChanges overrides and utility methods.
/// </summary>
public partial class QuestionnaireDbContext
{
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically updates CreatedAt and LastUpdatedAt timestamps for tracked entities.
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                if (entry.State == EntityState.Added)
                    user.CreatedAt = DateTime.UtcNow;
                user.LastUpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Questionnaire questionnaire)
            {
                if (entry.State == EntityState.Added)
                    questionnaire.CreatedAt = DateTime.UtcNow;
                questionnaire.LastUpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Assignment assignment)
            {
                if (entry.State == EntityState.Added)
                    assignment.CreatedAt = DateTime.UtcNow;
                assignment.LastUpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Response response)
            {
                if (entry.State == EntityState.Added)
                    response.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}