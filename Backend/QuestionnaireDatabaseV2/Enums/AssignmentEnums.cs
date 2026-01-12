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
/// Represents the type of assignment which affects visibility and participation rules.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssignmentType
{
    /// <summary>
    /// Anonymous assignment where participant identities are never shown in results.
    /// </summary>
    Anonymous,

    /// <summary>
    /// Peer assignment where participants can see each other's responses.
    /// </summary>
    Peer,

    /// <summary>
    /// Mixed-role assignment where different roles may have different visibility.
    /// </summary>
    MixedRole,

    /// <summary>
    /// Individual assignment for personal assessment or feedback.
    /// </summary>
    Individual
}
/// <summary>
/// Represents the permissions a participant can have within an assignment.
/// This enum uses the Flags attribute to allow bitwise combination of multiple permissions.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParticipantPermissions
{
    /// <summary>
    /// No permissions granted.
    /// </summary>
    None = 0,

    /// <summary>
    /// Can submit responses to the assignment.
    /// </summary>
    CanAnswer = 1,

    /// <summary>
    /// Can view their own submitted responses.
    /// </summary>
    CanViewOwnResults = 2,

    /// <summary>
    /// Can view responses from other participants.
    /// </summary>
    CanViewOtherResults = 4,

    /// <summary>
    /// Is not required to answer - participation is optional.
    /// </summary>
    OptionalParticipation = 8,

    /// <summary>
    /// Standard participant permissions - can answer and view own results.
    /// </summary>
    StandardParticipant = CanAnswer | CanViewOwnResults,

    /// <summary>
    /// Peer review permissions - can answer and view all results.
    /// </summary>
    PeerReviewer = CanAnswer | CanViewOwnResults | CanViewOtherResults,

    /// <summary>
    /// Observer permissions - can view all results but not required to answer.
    /// </summary>
    Observer = CanViewOwnResults | CanViewOtherResults | OptionalParticipation
}