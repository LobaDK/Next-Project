using API.DTO.Requests.Auth;
using API.DTO.Responses.Auth;
using API.Services;
using Settings.Models;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;
using QuestionnaireDatabaseV2.Enums;
using Microsoft.EntityFrameworkCore;
using QuestionnaireAPI.Services.Authentication;
using QuestionnaireAPI.Exceptions;
using QuestionnaireAPI.DTO.Responses.Auth;

namespace API.Controllers;

/// <summary>
/// Authentication controller for handling user login and authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user using their credentials and generates JWT tokens
    /// </summary>
    /// <param name="userLogin">The user login credentials</param>
    /// <returns>Authentication response with JWT tokens on success</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
    {
        try
        {
            var response = await _authService.AuthenticateAsync(userLogin.Username, userLogin.Password);
            
            if (response == null)
            {
                return Unauthorized("User information not found");
            }

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid username or password");
        }
        catch (AuthException.MaintenanceModeException ex)
        {
            _logger.LogWarning("Authentication attempt during maintenance mode: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "The system is currently in maintenance mode. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error for user {Username}", userLogin.Username);
            return StatusCode(StatusCodes.Status500InternalServerError, "Authentication service error");
        }
    }

    /// <summary>
    /// Check authentication status
    /// </summary>
    [HttpGet("status")]
    public IActionResult Status()
    {
        // Simple status endpoint - can be expanded if needed
        return Ok(new { Status = "Available" });
    }

    /// <summary>
    /// Simple ping endpoint for frontend health checks
    /// </summary>
    /// <returns>Pong response with timestamp</returns>
    [HttpHead("ping")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new {message = "Works"});
    }

    [HttpGet("permissions")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PermissionCode>), StatusCodes.Status200OK)]
    public IActionResult GetPermissions()
    {
        List<PermissionCode> permissions = [.. Enum.GetValues<UserPermissions>().
            Select(p => new PermissionCode{Name = p.ToString(), Value = (int)p})];

        return Ok(permissions);
    }
}
