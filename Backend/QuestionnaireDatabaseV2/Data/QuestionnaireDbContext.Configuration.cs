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
        ConfigureAssignmentViewerEntity(modelBuilder);
        ConfigureResponseEntity(modelBuilder);
        ConfigureEnumConversions(modelBuilder);
    }

    private void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(u => u.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure relationships
            entity.HasMany(u => u.CreatedAssignments)
                .WithOne(a => a.CreatedByUser)
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.AssignmentParticipants)
                .WithOne(ap => ap.User)
                .HasForeignKey(ap => ap.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.AssignmentViewers)
                .WithOne(av => av.User)
                .HasForeignKey(av => av.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureQuestionnaireEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Questionnaire>(entity =>
        {
            entity.Property(q => q.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(q => q.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure QuestionnaireVersion complex type
            entity.OwnsOne(q => q.Version, version =>
            {
                version.Property(v => v.CopiedFromQuestionnaireId)
                    .HasColumnName("CopiedFromQuestionnaireId");
                version.Property(v => v.CopiedFromTitle)
                    .HasColumnName("CopiedFromTitle")
                    .HasMaxLength(200);
            });

            entity.HasOne(q => q.CopiedFromQuestionnaire)
                .WithMany(q => q.CopiedQuestionnaires)
                .HasForeignKey("Version_CopiedFromQuestionnaireId")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(q => q.Responses)
                .WithOne(r => r.Questionnaire)
                .HasForeignKey(r => r.QuestionnaireId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureAssignmentEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(a => a.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(a => a.Participants)
                .WithOne(ap => ap.Assignment)
                .HasForeignKey(ap => ap.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Viewers)
                .WithOne(av => av.Assignment)
                .HasForeignKey(av => av.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Responses)
                .WithOne(r => r.Assignment)
                .HasForeignKey(r => r.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAssignmentParticipantEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignmentParticipant>(entity =>
        {
            entity.Property(ap => ap.AddedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(ap => ap.AddedByUser)
                .WithMany()
                .HasForeignKey(ap => ap.AddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureAssignmentViewerEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignmentViewer>(entity =>
        {
            // Currently no special configuration needed
        });
    }

    private void ConfigureResponseEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Response>(entity =>
        {
            entity.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(r => r.SubmittedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(r => r.Participant)
                .WithMany(u => u.Responses)
                .HasForeignKey(r => r.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Questionnaire)
                .WithMany(q => q.Responses)
                .HasForeignKey(r => r.QuestionnaireId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureEnumConversions(ModelBuilder modelBuilder)
    {
        // Configure enum conversions to strings for better readability
        modelBuilder.Entity<Questionnaire>()
            .Property(q => q.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Assignment>()
            .Property(a => a.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Assignment>()
            .Property(a => a.Status)
            .HasConversion<string>();
    }
}