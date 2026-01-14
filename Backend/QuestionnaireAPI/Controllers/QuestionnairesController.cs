using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireDatabaseV2;
using QuestionnaireDatabaseV2.Entities;
using QuestionnaireAPI.DTO.Requests.Questionnaire;
using QuestionnaireAPI.DTO.Responses.Questionnaire;

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
        public async Task<ActionResult<IEnumerable<QuestionnaireBaseDTO>>> GetQuestionnaires()
        {
            var questionnaires = await _context.Questionnaires
                .Where(q => !q.IsDeleted)
                .Select(q => new QuestionnaireBaseDTO
                {
                    Id = q.Id,
                    Title = q.Title,
                    Status = q.Status,
                    CreatedAt = q.CreatedAt,
                    LastUpdatedAt = q.LastUpdatedAt,
                    Version = new QuestionnaireVersionDTO
                    {
                        CopiedFromQuestionnaireId = q.CopiedFromQuestionnaireId,
                        CopiedFromTitle = q.CopiedFromTitle
                    },
                    IsDeleted = q.IsDeleted
                })
                .ToListAsync();

            return questionnaires;
        }

        /// <summary>
        /// Get questionnaire by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionnaireBaseDTO>> GetQuestionnaire(Guid id)
        {
            var questionnaire = await _context.Questionnaires
                .Where(q => q.Id == id && !q.IsDeleted)
                .Select(q => new QuestionnaireBaseDTO
                {
                    Id = q.Id,
                    Title = q.Title,
                    Status = q.Status,
                    CreatedAt = q.CreatedAt,
                    LastUpdatedAt = q.LastUpdatedAt,
                    Version = new QuestionnaireVersionDTO
                    {
                        CopiedFromQuestionnaireId = q.CopiedFromQuestionnaireId,
                        CopiedFromTitle = q.CopiedFromTitle
                    },
                    IsDeleted = q.IsDeleted
                })
                .FirstOrDefaultAsync();

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
        public async Task<ActionResult<QuestionnaireBaseDTO>> PostQuestionnaire(CreateQuestionnaireDTO createDto)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid or missing user ID in token");
            }

            var questionnaire = new Questionnaire
            {
                Title = createDto.Title,
                Description = createDto.Description,
                SchemaJson = createDto.SchemaJson,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Questionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();

            var responseDto = new QuestionnaireBaseDTO
            {
                Id = questionnaire.Id,
                Title = questionnaire.Title,
                Status = questionnaire.Status,
                CreatedAt = questionnaire.CreatedAt,
                LastUpdatedAt = questionnaire.LastUpdatedAt,
                Version = new QuestionnaireVersionDTO
                {
                    CopiedFromQuestionnaireId = questionnaire.CopiedFromQuestionnaireId,
                    CopiedFromTitle = questionnaire.CopiedFromTitle
                },
                IsDeleted = questionnaire.IsDeleted
            };

            return CreatedAtAction("GetQuestionnaire", new { id = questionnaire.Id }, responseDto);
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
            questionnaire.Status = QuestionnaireDatabaseV2.Enums.QuestionnaireStatus.Deleted;
            questionnaire.IsDeleted = true;
            questionnaire.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuestionnaireExists(Guid id)
        {
            return _context.Questionnaires.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}