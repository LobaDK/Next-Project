using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireDatabaseV2;

/// <summary>
/// Entity configuration partial class for QuestionnaireDbContext.
/// Contains all entity relationship and property configurations.
/// </summary>
public partial class QuestionnaireDbContext
{
    /// <summary>
    /// Configures all entity relationships, properties, and constraints.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    private void ConfigureEntities(ModelBuilder modelBuilder)
    {
        ConfigureUserEntity(modelBuilder);
        ConfigureQuestionnaireEntity(modelBuilder);
        ConfigureAssignmentEntity(modelBuilder);
        ConfigureAssignmentParticipantEntity(modelBuilder);
        ConfigureResponseEntity(modelBuilder);
        ConfigureEnumConversions(modelBuilder);
    }

    private void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // Only keeping delete behavior that can't be specified via attributes
            entity.HasMany(u => u.CreatedAssignments)
                .WithOne(a => a.CreatedByUser)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.AssignmentParticipants)
                .WithOne(ap => ap.User)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureQuestionnaireEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Questionnaire>(entity =>
        {
            // Self-referencing relationship for copied questionnaires
            entity.HasOne(q => q.CopiedFromQuestionnaire)
                .WithMany(q => q.CopiedQuestionnaires)
                .HasForeignKey(q => q.CopiedFromQuestionnaireId)
                .OnDelete(DeleteBehavior.Restrict);

            // Delete behavior for responses
            entity.HasMany(q => q.Responses)
                .WithOne(r => r.Questionnaire)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureAssignmentEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            // Only delete behavior (can't be specified via attributes)
            entity.HasMany(a => a.Participants)
                .WithOne(ap => ap.Assignment)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Responses)
                .WithOne(r => r.Assignment)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAssignmentParticipantEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignmentParticipant>(entity =>
        {
            // Only delete behavior for AddedByUser (complex relationship)
            entity.HasOne(ap => ap.AddedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureResponseEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Response>(entity =>
        {
            // Only delete behavior (can't be specified via attributes)
            entity.HasOne(r => r.Participant)
                .WithMany(u => u.Responses)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureEnumConversions(ModelBuilder modelBuilder)
    {
        // Enum string conversions are now handled via [Column(TypeName = "nvarchar(50)")] attributes
        // This method can be removed or kept for any custom enum conversions if needed
    }
}