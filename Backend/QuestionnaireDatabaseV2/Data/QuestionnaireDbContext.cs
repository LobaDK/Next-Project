using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireDatabaseV2;

/// <summary>
/// Entity Framework DbContext for the questionnaire system.
/// Provides access to all entities and configures relationships and constraints.
/// </summary>
public partial class QuestionnaireDbContext : DbContext
{
    public QuestionnaireDbContext() { }

    public QuestionnaireDbContext(DbContextOptions<QuestionnaireDbContext> options) : base(options) { }

    // DbSets for all entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Questionnaire> Questionnaires { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<AssignmentParticipant> AssignmentParticipants { get; set; } = null!;
    public DbSet<Response> Responses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureEntities(modelBuilder);
    }
}