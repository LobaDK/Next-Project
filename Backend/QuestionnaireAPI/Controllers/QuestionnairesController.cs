using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;

namespace QuestionnaireAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionnairesController : ControllerBase
    {
        private readonly QuestionnaireDbContext _context;

        public QuestionnairesController(QuestionnaireDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all questionnaires (excluding deleted)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questionnaire>>> GetQuestionnaires()
        {
            return await _context.Questionnaires
                .Where(q => !q.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Get questionnaire by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Questionnaire>> GetQuestionnaire(Guid id)
        {
            var questionnaire = await _context.Questionnaires
                .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);

            if (questionnaire == null)
            {
                return NotFound();
            }

            return questionnaire;
        }

        /// <summary>
        /// Create a new questionnaire
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Questionnaire>> PostQuestionnaire(Questionnaire questionnaire)
        {
            questionnaire.CreatedAt = DateTime.UtcNow;
            questionnaire.LastUpdatedAt = DateTime.UtcNow;
            questionnaire.IsDeleted = false;

            _context.Questionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestionnaire", new { id = questionnaire.Id }, questionnaire);
        }

        /// <summary>
        /// Update questionnaire
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestionnaire(Guid id, Questionnaire questionnaire)
        {
            if (id != questionnaire.Id)
            {
                return BadRequest();
            }

            var existingQuestionnaire = await _context.Questionnaires.FindAsync(id);
            if (existingQuestionnaire == null || existingQuestionnaire.IsDeleted)
            {
                return NotFound();
            }

            // Preserve some fields during update
            questionnaire.CreatedAt = existingQuestionnaire.CreatedAt;
            questionnaire.LastUpdatedAt = DateTime.UtcNow;
            questionnaire.IsDeleted = existingQuestionnaire.IsDeleted;

            _context.Entry(existingQuestionnaire).State = EntityState.Detached;
            _context.Entry(questionnaire).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionnaireExists(id))
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
        /// Soft delete questionnaire
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionnaire(Guid id)
        {
            var questionnaire = await _context.Questionnaires.FindAsync(id);
            if (questionnaire == null || questionnaire.IsDeleted)
            {
                return NotFound();
            }

            // Soft delete
            questionnaire.IsDeleted = true;
            questionnaire.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Copy questionnaire to create a new version
        /// </summary>
        [HttpPost("{id}/copy")]
        public async Task<ActionResult<Questionnaire>> CopyQuestionnaire(Guid id)
        {
            var originalQuestionnaire = await _context.Questionnaires
                .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);

            if (originalQuestionnaire == null)
            {
                return NotFound();
            }

            // Create copy
            var copiedQuestionnaire = new Questionnaire
            {
                Title = $"{originalQuestionnaire.Title} - Copy",
                Description = originalQuestionnaire.Description,
                SchemaJson = originalQuestionnaire.SchemaJson,
                Category = originalQuestionnaire.Category,
                CreatedByUserId = originalQuestionnaire.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // Set version information
            copiedQuestionnaire.Version = new QuestionnaireVersion
            {
                CopiedFromQuestionnaireId = originalQuestionnaire.Id,
                CopiedFromTitle = originalQuestionnaire.Title
            };

            _context.Questionnaires.Add(copiedQuestionnaire);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestionnaire", new { id = copiedQuestionnaire.Id }, copiedQuestionnaire);
        }

        private bool QuestionnaireExists(Guid id)
        {
            return _context.Questionnaires.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}