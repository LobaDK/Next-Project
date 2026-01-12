using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsesController : ControllerBase
    {
        private readonly QuestionnaireDbContext _context;

        public ResponsesController(QuestionnaireDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all responses
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Response>>> GetResponses()
        {
            return await _context.Responses
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Questionnaire)
                .Include(r => r.Participant)
                .ToListAsync();
        }

        /// <summary>
        /// Get response by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetResponse(Guid id)
        {
            var response = await _context.Responses
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Questionnaire)
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (response == null)
            {
                return NotFound();
            }

            return response;
        }

        /// <summary>
        /// Create a new response
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Response>> PostResponse(Response response)
        {
            response.CreatedAt = DateTime.UtcNow;

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResponse", new { id = response.Id }, response);
        }

        /// <summary>
        /// Update response
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResponse(Guid id, Response response)
        {
            if (id != response.Id)
            {
                return BadRequest();
            }

            var existingResponse = await _context.Responses.FindAsync(id);
            if (existingResponse == null)
            {
                return NotFound();
            }

            // Preserve created date
            response.CreatedAt = existingResponse.CreatedAt;
            
            // If response is being completed, set completion date
            if (existingResponse.SubmittedAt == null && response.SubmittedAt != null)
            {
                response.SubmittedAt = DateTime.UtcNow;
            }

            _context.Entry(existingResponse).State = EntityState.Detached;
            _context.Entry(response).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResponseExists(id))
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
        /// Delete response
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResponse(Guid id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            _context.Responses.Remove(response);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Get responses for a specific assignment
        /// </summary>
        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<Response>>> GetResponsesByAssignment(Guid assignmentId)
        {
            var responses = await _context.Responses
                .Include(r => r.Participant)
                .Where(r => r.AssignmentId == assignmentId)
                .ToListAsync();

            return responses;
        }

        /// <summary>
        /// Get responses by a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Response>>> GetResponsesByUser(Guid userId)
        {
            var responses = await _context.Responses
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Questionnaire)
                .Where(r => r.ParticipantId == userId)
                .ToListAsync();

            return responses;
        }

        /// <summary>
        /// Submit a response
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitResponse(Guid id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            if (response.SubmittedAt != null)
            {
                return BadRequest("Response is already submitted");
            }

            response.SubmittedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResponseExists(Guid id)
        {
            return _context.Responses.Any(e => e.Id == id);
        }
    }
}