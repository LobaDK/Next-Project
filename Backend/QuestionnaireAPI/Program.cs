using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using QuestionnaireDatabaseV2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework
builder.Services.AddDbContext<QuestionnaireDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Questionnaire API", Version = "v1" });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowedOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200", "http://10.0.1.5")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Access Policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"))
    .AddPolicy("DefaultUserOnly", policy => policy.RequireRole("defaultUser"))
    .AddPolicy("ManagerAndDefaultUserOnly", policy => policy.RequireRole("Manager", "defaultUser"));

var app = builder.Build();

// Ensure the database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<QuestionnaireDbContext>();
    if (context.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        int max_attempts = 3;
        
        for (int attempt = 0; attempt < max_attempts; attempt++)
        {
            if (context.Database.CanConnect())
            {
                context.Database.Migrate();
                break;
            }
            else if (!databaseCreator.Exists())
            {
                logger.LogInformation("Database does not exist, creating it...");
                databaseCreator.Create();
                context.Database.Migrate();
                break;
            }
            else
            {
                logger.LogWarning("Waiting for database to be created/migrated... ({attempt}/{max_attempts})", attempt + 1, max_attempts);
                Thread.Sleep(TimeSpan.FromSeconds(30));
                if (attempt == max_attempts - 1)
                {
                    logger.LogCritical("Database is not reachable, exiting.");
                    Environment.Exit(1);
                }
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthorization();
app.MapControllers();

app.Run();
