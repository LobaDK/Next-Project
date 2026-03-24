namespace UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IJwtService> _mockJwtService = new();
        private readonly Mock<IAuthenticationBridge> _mockAuthBridge = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["LDAP:RoleMappingsCN:Admin"] = "Admin",
                    ["LDAP:RoleMappingsCN:Teacher"] = "Teacher"
                })
                .Build();

            _loggerFactory = LoggerFactory.Create(_ => { });

            _service = new AuthService(
                _mockJwtService.Object,
                _mockAuthBridge.Object,
                _configuration,
                _mockUnitOfWork.Object,
                _loggerFactory);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithTokens()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };
            var userId = Guid.NewGuid();

            var ldapUser = new BasicUserInfoWithUserID
            {
                UserId = userId.ToString(),
                Name = "Test User",
                Username = "user",
                MemberOf = ["Admin"]
            };

            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password));
            _mockAuthBridge.Setup(a => a.IsConnected()).Returns(true);
            _mockAuthBridge.Setup(a => a.SearchUser<BasicUserInfoWithUserID>(login.Username)).Returns(ldapUser);

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(r => r.GetUserAsync(userId)).ReturnsAsync((FullUser?)null);
            _mockUnitOfWork.Setup(u => u.User).Returns(mockUserRepo.Object);

            _mockJwtService.Setup(s => s.GetAccessTokenClaims(It.IsAny<JWTUser>())).Returns([]);
            _mockJwtService.Setup(s => s.GetRefreshTokenClaims(It.IsAny<string>())).Returns([]);
            _mockJwtService.Setup(s => s.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns("access-token");
            _mockJwtService.Setup(s => s.GenerateRefreshToken(It.IsAny<IEnumerable<Claim>>())).Returns("refresh-token");

            // Act
            IActionResult result = await _service.Login(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthenticationResponse>(okResult.Value);
            Assert.Equal("access-token", response.AuthToken);
            Assert.Equal("refresh-token", response.RefreshToken);
            _mockAuthBridge.Verify(a => a.Dispose(), Times.Once);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorizedMessage()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "wrong" };
            _mockAuthBridge
                .Setup(a => a.Authenticate(login.Username, login.Password))
                .Throws(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            IActionResult result = await _service.Login(login);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorized.Value);
        }

        [Fact]
        public async Task Login_UserRoleCannotBeDetermined_ReturnsUnauthorized()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };
            var ldapUser = new BasicUserInfoWithUserID
            {
                UserId = Guid.NewGuid().ToString(),
                Name = "Test User",
                Username = "user",
                MemberOf = []
            };

            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password));
            _mockAuthBridge.Setup(a => a.IsConnected()).Returns(true);
            _mockAuthBridge.Setup(a => a.SearchUser<BasicUserInfoWithUserID>(login.Username)).Returns(ldapUser);

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(r => r.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync((FullUser?)null);
            _mockUnitOfWork.Setup(u => u.User).Returns(mockUserRepo.Object);

            // Act
            IActionResult result = await _service.Login(login);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _mockAuthBridge.Verify(a => a.Dispose(), Times.Once);
        }
    }
}
