using System.Text.Json.Serialization;

namespace QuestionnaireDatabaseV2.Enums;

/// <summary>
/// Represents the status of a questionnaire in its lifecycle.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestionnaireStatus
{
    /// <summary>
    /// Questionnaire is in draft state and not available for assignments.
    /// </summary>
    Draft,

    /// <summary>
    /// Questionnaire is published and available for creating assignments.
    /// </summary>
    Published,

    /// <summary>
    /// Questionnaire is archived and cannot be used for new assignments.
    /// </summary>
    Archived,

    /// <summary>
    /// Questionnaire has been permanently deleted (soft delete).
    /// </summary>
    Deleted
}