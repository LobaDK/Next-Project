using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireAPI.DTO.Responses.Questionnaire;

/// <summary>
/// Version information for questionnaire tracking copy relationships.
/// </summary>
public class QuestionnaireVersionDTO
{
    /// <summary>
    /// Gets or sets the original questionnaire this was copied from.
    /// </summary>
    public Guid? CopiedFromQuestionnaireId { get; set; }

    /// <summary>
    /// Gets or sets the title of the original questionnaire this was copied from.
    /// </summary>
    public string? CopiedFromTitle { get; set; }
}

/// <summary>
/// Base DTO for questionnaire responses without navigation properties.
/// </summary>
public class QuestionnaireBaseDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for this questionnaire.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the questionnaire.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of this questionnaire.
    /// </summary>
    public QuestionnaireStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when this questionnaire was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when this questionnaire was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets version information for this questionnaire.
    /// </summary>
    public QuestionnaireVersionDTO? Version { get; set; }

    /// <summary>
    /// Gets or sets whether this questionnaire has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}