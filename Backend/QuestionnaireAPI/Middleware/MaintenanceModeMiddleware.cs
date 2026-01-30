using QuestionnaireAPI.Interfaces;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.Middleware;

public class MaintenanceModeMiddleware(RequestDelegate next, IMaintenanceMonitor maintenanceMonitor, ILogger<MaintenanceModeMiddleware> logger, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IMaintenanceMonitor _maintenanceMonitor = maintenanceMonitor;
    private readonly ILogger<MaintenanceModeMiddleware> _logger = logger;
    private readonly SystemSettings _systemSettings = ConfigurationBinderService.Bind<SystemSettings>(configuration);

    public async Task InvokeAsync(HttpContext context)
    {
        if (_maintenanceMonitor.IsMaintenanceEnabled)
        {
            if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
            {
                UserRole? role = ExtractUserRoleFromClaims(context.User);

                if (role is null || role != UserRole.Admin)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("The system is currently under maintenance. Please try again later.");
                    return;
                }
            }
            else if (context.Request.Path.Value == "/api/auth/login" && context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();
                var username = form["username"].ToString();

                if (string.Equals(username, _systemSettings.AdminUsername, StringComparison.OrdinalIgnoreCase) == false)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("The system is currently under maintenance. Please try again later.");
                    return;
                }
            }
        }

        await _next(context);
    }

    private UserRole? ExtractUserRoleFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var userRoleClaim = claimsPrincipal.FindFirst("role");
        
        if (userRoleClaim == null || Enum.TryParse<UserRole>(userRoleClaim.Value, out var role) == false)
        {
            _logger.LogWarning("User role claim not found or invalid in token");
            return null;
        }

        return role;
    }
}
