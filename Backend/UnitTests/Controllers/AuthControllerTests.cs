namespace UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService = new();
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _controller = new AuthController(_mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Login_DelegatesToService_ReturnsServiceResult()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };
            var expected = new OkObjectResult(new AuthenticationResponse
            {
                AuthToken = "access-token",
                RefreshToken = "refresh-token"
            });

            _mockAuthService
                .Setup(s => s.Login(login))
                .ReturnsAsync(expected);

            // Act
            IActionResult result = await _controller.Login(login);

            // Assert
            Assert.Same(expected, result);
            _mockAuthService.Verify(s => s.Login(login), Times.Once);
        }

        [Fact]
        public async Task Refresh_DelegatesToService_WithUserAndHeader()
        {
            // Arrange
            var request = new RefreshRequest { ExpiredToken = "expired-token" };
            var expected = new UnauthorizedResult();
            const string authHeader = "Bearer refresh-token";

            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = authHeader;
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim("sub", "user-id")], "test"));

            _mockAuthService
                .Setup(s => s.Refresh(request, _controller.User, authHeader))
                .ReturnsAsync(expected);

            // Act
            IActionResult result = await _controller.Refresh(request);

            // Assert
            Assert.Same(expected, result);
            _mockAuthService.Verify(s => s.Refresh(request, _controller.User, authHeader), Times.Once);
        }

        [Fact]
        public async Task Logout_DelegatesToService_WithHeader()
        {
            // Arrange
            var expected = new OkResult();
            const string authHeader = "Bearer refresh-token";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = authHeader;

            _mockAuthService
                .Setup(s => s.Logout(authHeader))
                .ReturnsAsync(expected);

            // Act
            IActionResult result = await _controller.Logout();

            // Assert
            Assert.Same(expected, result);
            _mockAuthService.Verify(s => s.Logout(authHeader), Times.Once);
        }

        [Fact]
        public void WhoAmI_DelegatesToService_WithHeader()
        {
            // Arrange
            var expected = new OkObjectResult(new JWTUser
            {
                Guid = Guid.NewGuid(),
                Username = "test-user",
                Name = "Test User",
                Role = "Admin",
                Permissions = 1
            });
            const string authHeader = "Bearer access-token";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = authHeader;

            _mockAuthService
                .Setup(s => s.WhoAmI(authHeader))
                .Returns(expected);

            // Act
            IActionResult result = _controller.WhoAmI();

            // Assert
            Assert.Same(expected, result);
            _mockAuthService.Verify(s => s.WhoAmI(authHeader), Times.Once);
        }
    }
}
