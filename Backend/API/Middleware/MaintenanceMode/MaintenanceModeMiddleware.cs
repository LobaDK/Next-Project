namespace API.Middleware.MaintenanceMode;

public class MaintenanceModeMiddleware(RequestDelegate next, IMaintenanceMonitor maintenanceMonitor, ILogger<MaintenanceModeMiddleware> logger, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IMaintenanceMonitor _maintenanceMonitor = maintenanceMonitor;
    private readonly ILogger<MaintenanceModeMiddleware> _logger = logger;
    private readonly SystemSettings _systemSettings = ConfigurationBinderService.Bind<SystemSettings>(configuration);
    private readonly List<string> _allowedEndpoints = ["/api/system/*"];

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
            else if (context.Request.Path.Value == "/api/auth" && context.Request.HasFormContentType)
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
            else if (context.Request.Path.Value is not null)
            {
                // Allow access to specific endpoints even during maintenance
                var path = context.Request.Path.Value;
                foreach (string allowedEndpoint in _allowedEndpoints)
                {
                    if (allowedEndpoint.Equals(path, StringComparison.OrdinalIgnoreCase))
                    {
                        await _next(context);
                        return;
                    }
                    else if (allowedEndpoint.EndsWith("*", StringComparison.Ordinal))
                    {
                        var wildcardBase = allowedEndpoint.TrimEnd('*').TrimEnd('/');

                        if (path.Equals(wildcardBase, StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWith(wildcardBase + "/", StringComparison.OrdinalIgnoreCase))
                        {
                            await _next(context);
                            return;
                        }
                    }
                }

                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("The system is currently under maintenance. Please try again later.");
                return;
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

        if (userRoleClaim == null)
        {
            _logger.LogWarning("User role claim not found or invalid in token");
            return null;
        }

        if (Enum.TryParse<UserRoles>(userRoleClaim.Value, true, out var role) == false)
        {
            _logger.LogWarning("User role claim value '{claimValue}' could not be parsed to a valid UserRoles enum.", userRoleClaim.Value);
            return null;
        }

        return role;
    }
}
