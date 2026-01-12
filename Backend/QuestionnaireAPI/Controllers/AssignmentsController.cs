using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly QuestionnaireDbContext _context;

        public AssignmentsController(QuestionnaireDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all assignments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
            return await _context.Assignments
                .Include(a => a.Questionnaire)
                .Include(a => a.Participants)
                .Include(a => a.Viewers)
                .ToListAsync();
        }

        /// <summary>
        /// Get assignment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignment(Guid id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Questionnaire)
                .Include(a => a.Participants)
                .Include(a => a.Viewers)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            return assignment;
        }

        /// <summary>
        /// Create a new assignment
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Assignment>> PostAssignment(Assignment assignment)
        {
            assignment.CreatedAt = DateTime.UtcNow;
            assignment.LastUpdatedAt = DateTime.UtcNow;

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssignment", new { id = assignment.Id }, assignment);
        }

        /// <summary>
        /// Update assignment
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignment(Guid id, Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return BadRequest();
            }

            var existingAssignment = await _context.Assignments.FindAsync(id);
            if (existingAssignment == null)
            {
                return NotFound();
            }

            // Preserve created date
            assignment.CreatedAt = existingAssignment.CreatedAt;
            assignment.LastUpdatedAt = DateTime.UtcNow;

            _context.Entry(existingAssignment).State = EntityState.Detached;
            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
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
        /// Delete assignment
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssignmentExists(Guid id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}