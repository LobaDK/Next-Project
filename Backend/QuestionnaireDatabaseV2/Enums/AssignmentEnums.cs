using System.Text.Json.Serialization;

namespace QuestionnaireDatabaseV2.Enums;

/// <summary>
/// Represents the status of an assignment in its lifecycle.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssignmentStatus
{
    /// <summary>
    /// Assignment is being configured and not yet active.
    /// </summary>
    Draft,

    /// <summary>
    /// Assignment is active and participants can submit responses.
    /// </summary>
    Active,

    /// <summary>
    /// Assignment is closed - no new responses accepted but results are viewable.
    /// </summary>
    Closed,

    /// <summary>
    /// Assignment is archived for long-term storage.
    /// </summary>
    Archived,

    /// <summary>
    /// Assignment has been cancelled and is no longer valid.
    /// </summary>
    Cancelled
}

/// <summary>
/// Represents participant permissions for assignments.
/// Flags that define what a participant can do within an assignment.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParticipantPermissions
{
    /// <summary>
    /// No permissions - participant cannot access assignment.
    /// </summary>
    None = 0,

    /// <summary>
    /// Participant can answer questions in the assignment.
    /// </summary>
    CanAnswer = 1,

    /// <summary>
    /// Participant can view their own results after completing the assignment.
    /// </summary>
    CanViewOwnResults = 2,

    /// <summary>
    /// Participant can view other participants' results.
    /// </summary>
    CanViewOthersResults = 4,

    /// <summary>
    /// Combination: Can answer and view own results (typical for individual assignments).
    /// </summary>
    CanAnswerAndViewOwn = CanAnswer | CanViewOwnResults,

    /// <summary>
    /// Combination: Can answer and view all results (typical for peer review assignments).
    /// </summary>
    CanAnswerAndViewAll = CanAnswer | CanViewOwnResults | CanViewOthersResults,

    /// <summary>
    /// Read-only access: Can view own and others' results but cannot answer.
    /// </summary>
    ViewOnlyAll = CanViewOwnResults | CanViewOthersResults
}