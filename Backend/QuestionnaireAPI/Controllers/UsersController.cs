using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly QuestionnaireDbContext _context;

        public UsersController(QuestionnaireDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Get user by Active Directory GUID
        /// </summary>
        [HttpGet("by-ad-guid/{adGuid}")]
        public async Task<ActionResult<User>> GetUserByAdGuid(Guid adGuid)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ActiveDirectoryGuid == adGuid);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Get users by role
        /// </summary>
        [HttpGet("by-role/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string role)
        {
            if (!Enum.TryParse<UserRole>(role, true, out var userRole))
            {
                return BadRequest($"Invalid role. Valid roles are: {string.Join(", ", Enum.GetNames<UserRole>())}");
            }

            var users = await _context.Users
                .Where(u => u.Role == userRole)
                .ToListAsync();

            return users;
        }

        /// <summary>
        /// Get managers only
        /// </summary>
        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<User>>> GetManagers()
        {
            var managers = await _context.Users
                .Where(u => u.Role == UserRole.Manager)
                .ToListAsync();

            return managers;
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}