namespace API.Middleware.MaintenanceMode;

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
                UserRoles? role = ExtractUserRoleFromClaims(context.User);

                if (role is null || role != UserRoles.Admin)
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
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
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    await context.Response.WriteAsync("The system is currently under maintenance. Please try again later.");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("The system is currently under maintenance. Please try again later.");
                return;
            }
        }

        await _next(context);
    }

    private UserRoles? ExtractUserRoleFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var userRoleClaim = claimsPrincipal.FindFirst("role");
        
        if (userRoleClaim == null || Enum.TryParse<UserRoles>(userRoleClaim.Value, out var role) == false)
        {
            _logger.LogWarning("User role claim not found or invalid in token");
            return null;
        }

        return role;
    }
}
