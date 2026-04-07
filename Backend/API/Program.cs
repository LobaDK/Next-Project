using API.Middleware.MaintenanceMode;
using API.Middleware.RateLimiter;

const string settingsFile = "config.json";

var builder = WebApplication.CreateBuilder(args);

// Bootstrap logger for settings helper before the main logging pipeline is configured
using var bootstrapLoggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
ILogger<SettingsHelper> settingsLogger = bootstrapLoggerFactory.CreateLogger<SettingsHelper>();

SettingsHelper settingsHelper = new(settingsFile, settingsLogger);

if (!settingsHelper.SettingsExists())
{
    settingsHelper.CreateDefault();
}
else
{
    settingsHelper.CheckSettingsVersion();
}

builder.Configuration.AddJsonFile(settingsFile, optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddDBLogger(configure => builder.Configuration.GetSection("Logging:DBLogger"));

DatabaseSettings databaseSettings = ConfigurationBinderService.Bind<DatabaseSettings>(builder.Configuration);
JWTSettings jWTSettings = ConfigurationBinderService.Bind<JWTSettings>(builder.Configuration);
SystemSettings systemSettings = ConfigurationBinderService.Bind<SystemSettings>(builder.Configuration);
LoggerSettings loggerSettings = ConfigurationBinderService.Bind<LoggerSettings>(builder.Configuration);

Serilog.Core.Logger seriLogger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

builder.Logging.AddSerilog(seriLogger);
bootstrapLoggerFactory.AddSerilog(seriLogger);

// Add services to the container.

if (File.Exists("USEMOCKAUTH") || File.Exists("USEMOCKAUTH.txt"))
{
    builder.Services.AddScoped<IAuthenticationBridge, MockedAuthenticationBridge>();
    seriLogger.Warning("Using MOCK authentication bridge, this should NOT be used in production!");
}
else
{
    builder.Services.AddScoped<IAuthenticationBridge, ActiveDirectoryAuthenticationBridge>();
}

builder.Services.AddSingleton<IMaintenanceMonitor, MaintenanceMonitor>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<QuestionnaireTemplateAdd>, CreateQuestionnaireTemplateSubmissionValidator>();
builder.Services.AddScoped<JsonSerializerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IQuestionnaireTemplateService, QuestionnaireTemplateService>();
builder.Services.AddScoped<IActiveQuestionnaireService, ActiveQuestionnaireService>();
builder.Services.AddScoped<ISystemControllerService, SystemControllerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(cfg =>
{
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("AccessToken", x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = JwtService.GetAccessTokenValidationParameters(jWTSettings.AccessTokenSecret, issuer: jWTSettings.Issuer, audience: jWTSettings.Audience);

    // ASP.NET likes to map JWT claim names to their own URL schema claims
    // making it difficult to work with incoming tokens. This disables that.
    x.MapInboundClaims = false;
}).AddJwtBearer("RefreshToken", x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = JwtService.GetRefreshTokenValidationParameters(jWTSettings.RefreshTokenSecret);

    // ASP.NET likes to map JWT claim names to their own URL schema claims
    // making it difficult to work with incoming tokens. This disables that.
    x.MapInboundClaims = false;
});

builder.Services.Configure<RouteOptions>(o =>
{
    o.LowercaseUrls = true;
    o.LowercaseQueryStrings = true;
});

// Repositories
builder.Services.AddScoped<IQuestionnaireGroupRepository, QuestionnaireGroupRepository>();
builder.Services.AddScoped<IQuestionnaireTemplateRepository, QuestionnaireTemplateRepository>();
builder.Services.AddScoped<IActiveQuestionnaireRepository, ActiveQuestionnaireRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITrackedRefreshTokenRepository, TrackedRefreshTokenRepository>();
builder.Services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
}).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NEXT questionnaire API", Version = "v1" });

    options.UseAllOfToExtendReferenceSchemas();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token in the field below."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<Context>(o =>
    o.UseSqlServer(databaseSettings.ConnectionString,
        options =>
        {
            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));

// We have to configure Kestrel before building the app instance
string environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
if (environment != "Development")
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        IPAddress? address = null;
        if (!string.IsNullOrEmpty(systemSettings.ListenIP))
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(systemSettings.ListenIP);
            address = hostEntry.AddressList.SingleOrDefault(host => host.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ?? IPAddress.Loopback;
        }

        if (address is not null)
        {
            options.Listen(address, systemSettings.HttpPort);
        }
        else
        {
            options.ListenAnyIP(systemSettings.HttpPort);
        }

        if (systemSettings.UseSSL)
        {
            if (address is not null)
            {
                options.Listen(address, systemSettings.HttpsPort, listenOptions =>
                {
                    listenOptions.UseHttps(systemSettings.PfxCertificatePath);
                });
            }
            else
            {
                options.ListenAnyIP(systemSettings.HttpsPort, listenOptions =>
                {
                    listenOptions.UseHttps(systemSettings.PfxCertificatePath);
                });
            }
        }
    });
}

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowedOrigins",
        policy =>
        {
            policy.WithOrigins("http://10.0.1.5")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Access Policies
builder.Services.AddAuthorizationBuilder()
                      .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
                      .AddPolicy("TeacherOnly", policy => policy.RequireRole("teacher"))
                      .AddPolicy("StudentOnly", policy => policy.RequireRole("student"))
                      .AddPolicy("AdminAndTeacherOnly", policy => policy.RequireRole("admin", "teacher"))
                      .AddPolicy("StudentAndTeacherOnly", policy => policy.RequireRole("student", "teacher"));

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("global", new GlobalRateLimiterPolicy(bootstrapLoggerFactory.CreateLogger<GlobalRateLimiterPolicy>(), builder.Configuration));
});

var app = builder.Build();

app.UseCors("AllowedOrigins");

// Ensure the database is created and migrated
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    Context context = services.GetRequiredService<Context>();
    if (context.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        int max_attempts = 3;
        string failureReason = string.Empty;

        try
        {
            for (int attempt = 0; attempt < max_attempts; attempt++)
            {
                if (context.Database.CanConnect())
                {
                    failureReason = "Failed to migrate database.";
                    context.Database.Migrate();
                    break;
                }
                else if (!context.Database.CanConnect() && !databaseCreator.Exists())
                {
                    failureReason = "Failed to create database.";
                    logger.LogInformation("Database does not exist, creating it...");
                    databaseCreator.Create();
                    failureReason = "Failed to migrate database after creating it";
                    context.Database.Migrate();
                    break;
                }
                else
                {
                    logger.LogWarning("Waiting for database to be created/migrated... ({attempt}/{max_attempts})", attempt + 1, max_attempts);
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    if (attempt == max_attempts - 1)
                    {
                        failureReason = "Failed to reach/connect to database.";
                        throw new InvalidOperationException();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, failureReason);
            services.GetRequiredService<IMaintenanceMonitor>().EnableMaintenance(failureReason);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

if (systemSettings.UseSSL)
{
    app.UseHttpsRedirection();
}


app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();

app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("global");

app.UseMiddleware<MaintenanceModeMiddleware>();

app.Run();
