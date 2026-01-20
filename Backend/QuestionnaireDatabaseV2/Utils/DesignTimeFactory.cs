
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QuestionnaireDatabaseV2;

namespace Database;

/// <summary>
/// Design-time factory for creating instances of the database context.
/// This factory is used by Entity Framework tools (such as migrations) to create
/// a DbContext instance at design time when the application is not running.
/// </summary>
/// <remarks>
/// The factory reads configuration from the config.json file located in the API project
/// directory and configures the context to use SQL Server as the database provider.
/// </remarks>
public class DesignTimeFactory : IDesignTimeDbContextFactory<QuestionnaireDbContext>
{
    public QuestionnaireDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../API"))
            .AddJsonFile("config.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<QuestionnaireDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetSection("Database").GetSection("ConnectionString").Value);

        return new QuestionnaireDbContext(optionsBuilder.Options);
    }
}
