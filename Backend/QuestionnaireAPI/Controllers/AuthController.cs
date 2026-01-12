namespace API.Controllers;

/// <summary>
/// Authentication controller for handling user login and authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationBridge _authenticationBridge;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationBridge authenticationBridge, ILogger<AuthController> logger)
    {
        _authenticationBridge = authenticationBridge;
        _logger = logger;
    }

    /// <summary>
    /// Basic authentication endpoint
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        // TODO: Implement login logic
        return Ok("Authentication endpoint - implementation pending");
    }

    /// <summary>
    /// Check authentication status
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var isConnected = _authenticationBridge.IsConnected();
        return Ok(new { IsConnected = isConnected });
    }
}