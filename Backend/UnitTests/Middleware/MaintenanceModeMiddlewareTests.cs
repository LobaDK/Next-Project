using System.IO;
using System.Text;
using API.Middleware.MaintenanceMode;

namespace UnitTests.Middleware
{
    public class MaintenanceModeMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_MaintenanceDisabled_CallsNext()
        {
            // Arrange
            var context = CreateHttpContext();
            var nextCalled = false;
            var middleware = CreateMiddleware(isMaintenanceEnabled: false, onNext: () => nextCalled = true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_MaintenanceEnabled_AuthenticatedAdmin_CallsNext()
        {
            // Arrange
            var context = CreateHttpContext();
            context.User = CreateAuthenticatedUser(UserRoles.Admin.ToString());

            var nextCalled = false;
            var middleware = CreateMiddleware(isMaintenanceEnabled: true, onNext: () => nextCalled = true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_MaintenanceEnabled_AuthenticatedNonAdmin_ReturnsUnauthorized()
        {
            // Arrange
            var context = CreateHttpContext();
            context.User = CreateAuthenticatedUser(UserRoles.Teacher.ToString());

            var nextCalled = false;
            var middleware = CreateMiddleware(isMaintenanceEnabled: true, onNext: () => nextCalled = true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("The system is currently under maintenance. Please try again later.", await ReadResponseBodyAsync(context));
        }

        [Fact]
        public async Task InvokeAsync_MaintenanceEnabled_AuthenticatedWithoutRole_ReturnsUnauthorized()
        {
            // Arrange
            var context = CreateHttpContext();
            context.User = CreateAuthenticatedUser();

            var nextCalled = false;
            var middleware = CreateMiddleware(isMaintenanceEnabled: true, onNext: () => nextCalled = true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("The system is currently under maintenance. Please try again later.", await ReadResponseBodyAsync(context));
        }

        [Fact]
        public async Task InvokeAsync_MaintenanceEnabled_LoginWithAdminUsername_CallsNext()
        {
            // Arrange
            var context = CreateLoginRequestContext("ADMIN");
            var nextCalled = false;

            var middleware = CreateMiddleware(
                isMaintenanceEnabled: true,
                onNext: () => nextCalled = true,
                adminUsername: "admin");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_MaintenanceEnabled_LoginWithNonAdminUsername_ReturnsUnauthorized()
        {
            // Arrange
            var context = CreateLoginRequestContext("some-user");
            var nextCalled = false;

            var middleware = CreateMiddleware(
                isMaintenanceEnabled: true,
                onNext: () => nextCalled = true,
                adminUsername: "admin");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("The system is currently under maintenance. Please try again later.", await ReadResponseBodyAsync(context));
        }

        [Fact]
        public async Task InvokeAsync_MaintenanceEnabled_UnauthenticatedNonLoginRequest_ReturnsServiceUnavailable()
        {
            // Arrange
            var context = CreateHttpContext();
            context.Request.Path = "/api/users";

            var nextCalled = false;
            var middleware = CreateMiddleware(isMaintenanceEnabled: true, onNext: () => nextCalled = true);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("The system is currently under maintenance. Please try again later.", await ReadResponseBodyAsync(context));
        }

        private static MaintenanceModeMiddleware CreateMiddleware(bool isMaintenanceEnabled, Action onNext, string adminUsername = "admin")
        {
            Task Next(HttpContext _)
            {
                onNext();
                return Task.CompletedTask;
            }

            var maintenanceMonitor = new Mock<IMaintenanceMonitor>();
            maintenanceMonitor.Setup(m => m.IsMaintenanceEnabled).Returns(isMaintenanceEnabled);

            var logger = Mock.Of<ILogger<MaintenanceModeMiddleware>>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["System:AdminUsername"] = adminUsername
                })
                .Build();

            return new MaintenanceModeMiddleware(Next, maintenanceMonitor.Object, logger, config);
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static DefaultHttpContext CreateLoginRequestContext(string username)
        {
            var context = CreateHttpContext();
            context.Request.Path = "/api/auth/login";
            context.Request.Method = HttpMethods.Post;
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var body = $"username={Uri.EscapeDataString(username)}&password=ignored";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            return context;
        }

        private static ClaimsPrincipal CreateAuthenticatedUser(string? role = null)
        {
            var claims = new List<Claim>();

            if (role is not null)
            {
                claims.Add(new Claim("role", role));
            }

            var identity = new ClaimsIdentity(claims, authenticationType: "test");
            return new ClaimsPrincipal(identity);
        }

        private static async Task<string> ReadResponseBodyAsync(HttpContext context)
        {
            context.Response.Body.Position = 0;

            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }
    }
}
