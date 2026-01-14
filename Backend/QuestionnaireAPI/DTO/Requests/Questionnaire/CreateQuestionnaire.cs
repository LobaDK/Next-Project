using System.ComponentModel.DataAnnotations;
using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.DTO.Requests.Questionnaire;

/// <summary>
/// DTO for creating a new questionnaire.
/// </summary>
public class CreateQuestionnaireDTO
{
    /// <summary>
    /// Gets or sets the title of the questionnaire.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the questionnaire.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the JSON schema definition for this questionnaire.
    /// Contains the questions, options, validation rules, etc.
    /// </summary>
    [Required]
    public string SchemaJson { get; set; } = string.Empty;
}