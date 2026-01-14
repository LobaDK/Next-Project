using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;
using QuestionnaireDatabaseV2.Enums;
using QuestionnaireAPI.Services.User;
using QuestionnaireAPI.DTO.Responses.User;
using QuestionnaireAPI.DTO.Responses;
using QuestionnaireAPI.DTO.Requests;

namespace QuestionnaireAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly QuestionnaireDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            QuestionnaireDbContext context,
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current authenticated user's information
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            try
            {
                var currentUser = await _userService.GetCurrentUserAsync(User);
                
                if (currentUser == null)
                {
                    return NotFound("User not found");
                }

                return Ok(currentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user information");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Search and paginate users by name and role
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<UserDTO>>> SearchUsers([FromQuery] UserSearchRequest request)
        {
            try
            {
                var result = await _userService.SearchUsersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}