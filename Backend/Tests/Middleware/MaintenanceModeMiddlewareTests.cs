using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using QuestionnaireAPI.Interfaces;
using QuestionnaireAPI.Middleware;
using Microsoft.AspNetCore.Http.Features;

namespace Tests.Middleware;

public class MaintenanceModeMiddlewareTests
{
    private readonly Mock<IMaintenanceMonitor> _maintenanceMonitorMock;
    private readonly Mock<ILogger<MaintenanceModeMiddleware>> _loggerMock;
    private readonly IConfiguration _configuration;
    private readonly RequestDelegate _nextDelegate;

    public MaintenanceModeMiddlewareTests()
    {
        _maintenanceMonitorMock = new Mock<IMaintenanceMonitor>();
        _loggerMock = new Mock<ILogger<MaintenanceModeMiddleware>>();
        
        // Create proper configuration with SystemSettings section
        var configDict = new Dictionary<string, string?>
        {
            { "SystemSettings:AdminUsername", "admin" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        _nextDelegate = new RequestDelegate((HttpContext ctx) =>
        {
            ctx.Items["next_called"] = true;
            return Task.CompletedTask;
        });
    }

    private MaintenanceModeMiddleware CreateMiddleware()
    {
        return new MaintenanceModeMiddleware(
            _nextDelegate,
            _maintenanceMonitorMock.Object,
            _loggerMock.Object,
            _configuration
        );
    }

    private static HttpContext CreateHttpContext(
        bool isAuthenticated = false,
        string? role = null,
        string path = "/",
        bool hasForm = false,
        Dictionary<string, string>? formData = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;

        if (isAuthenticated)
        {
            var claims = new List<Claim>();
            if (role != null)
                claims.Add(new Claim("role", role));
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            context.User = new ClaimsPrincipal(identity);
        }
        else
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity());
        }

        if (hasForm && formData != null)
        {
            var formFields = new Dictionary<string, StringValues>();
            foreach (var kvp in formData)
            {
                formFields[kvp.Key] = new StringValues(kvp.Value);
            }
            var formCollection = new FormCollection(formFields);
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Method = "POST";
            
            // Mock ReadFormAsync
            var feature = new FormFeature(formCollection);
            context.Features.Set<IFormFeature>(feature);
        }

        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> GetResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public async Task MaintenanceOff_CallsNext()
    {
        _maintenanceMonitorMock.Setup(m => m.IsMaintenanceEnabled).Returns(false);
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        Assert.True(context.Items.ContainsKey("next_called"));
    }

    [Fact]
    public async Task MaintenanceOn_AuthenticatedAdmin_CallsNext()
    {
        _maintenanceMonitorMock.Setup(m => m.IsMaintenanceEnabled).Returns(true);
        var middleware = CreateMiddleware();
        var context = CreateHttpContext(isAuthenticated: true, role: "Admin");

        await middleware.InvokeAsync(context);

        Assert.True(context.Items.ContainsKey("next_called"));
    }

    [Fact]
    public async Task MaintenanceOn_AuthenticatedNonAdmin_Returns401()
    {
        _maintenanceMonitorMock.Setup(m => m.IsMaintenanceEnabled).Returns(true);
        var middleware = CreateMiddleware();
        var context = CreateHttpContext(isAuthenticated: true, role: "User");

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        var body = await GetResponseBody(context);
        Assert.Contains("under maintenance", body);
        Assert.False(context.Items.ContainsKey("next_called"));
    }

    [Fact]
    public async Task MaintenanceOn_LoginRequest_AdminUsername_CallsNext()
    {
        _maintenanceMonitorMock.Setup(m => m.IsMaintenanceEnabled).Returns(true);
        var middleware = CreateMiddleware();
        var formData = new Dictionary<string, string> { { "username", "admin" } };
        var context = CreateHttpContext(
            isAuthenticated: false,
            path: "/api/auth/login",
            hasForm: true,
            formData: formData
        );

        await middleware.InvokeAsync(context);

        Assert.True(context.Items.ContainsKey("next_called"));
    }

    [Fact]
    public async Task MaintenanceOn_LoginRequest_NonAdminUsername_Returns401()
    {
        _maintenanceMonitorMock.Setup(m => m.IsMaintenanceEnabled).Returns(true);
        var middleware = CreateMiddleware();
        var formData = new Dictionary<string, string> { { "username", "user" } };
        var context = CreateHttpContext(
            isAuthenticated: false,
            path: "/api/auth/login",
            hasForm: true,
            formData: formData
        );

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        var body = await GetResponseBody(context);
        Assert.Contains("under maintenance", body);
        Assert.False(context.Items.ContainsKey("next_called"));
    }

    [Fact]
    public async Task MaintenanceOn_UnauthenticatedNonLogin_Returns503()
    {
        _maintenanceMonitorMock.Setup(m => m.IsMaintenanceEnabled).Returns(true);
        var middleware = CreateMiddleware();
        var context = CreateHttpContext(
            isAuthenticated: false
        );

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
    }
}

// Helper class to mock form feature
internal class FormFeature(IFormCollection form) : IFormFeature
{
    public bool HasFormContentType => true;
    public IFormCollection? Form { get; set; } = form;

    public IFormCollection ReadForm() => Form!;
    public Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken) => Task.FromResult(Form!);
}