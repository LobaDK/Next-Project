using QuestionnaireDatabaseV2.Enums;

namespace QuestionnaireDatabaseV2.Utils;

/// <summary>
/// Utility class for working with participant permissions in assignments.
/// Provides helper methods to check and manipulate permission flags.
/// </summary>
public static class ParticipantPermissionsHelper
{
    /// <summary>
    /// Checks if the participant has permission to answer questions.
    /// </summary>
    /// <param name="permissions">The participant's permissions.</param>
    /// <returns>True if the participant can answer questions.</returns>
    public static bool CanAnswer(this ParticipantPermissions permissions)
        => permissions.HasFlag(ParticipantPermissions.CanAnswer);

    /// <summary>
    /// Checks if the participant has permission to view their own results.
    /// </summary>
    /// <param name="permissions">The participant's permissions.</param>
    /// <returns>True if the participant can view their own results.</returns>
    public static bool CanViewOwnResults(this ParticipantPermissions permissions)
        => permissions.HasFlag(ParticipantPermissions.CanViewOwnResults);

    /// <summary>
    /// Checks if the participant has permission to view others' results.
    /// </summary>
    /// <param name="permissions">The participant's permissions.</param>
    /// <returns>True if the participant can view others' results.</returns>
    public static bool CanViewOthersResults(this ParticipantPermissions permissions)
        => permissions.HasFlag(ParticipantPermissions.CanViewOthersResults);

    /// <summary>
    /// Checks if the participant has any viewing permissions.
    /// </summary>
    /// <param name="permissions">The participant's permissions.</param>
    /// <returns>True if the participant can view any results.</returns>
    public static bool CanViewAnyResults(this ParticipantPermissions permissions)
        => permissions.CanViewOwnResults() || permissions.CanViewOthersResults();

    /// <summary>
    /// Checks if the participant is read-only (can view but not answer).
    /// </summary>
    /// <param name="permissions">The participant's permissions.</param>
    /// <returns>True if the participant is read-only.</returns>
    public static bool IsReadOnly(this ParticipantPermissions permissions)
        => permissions.CanViewAnyResults() && !permissions.CanAnswer();

    /// <summary>
    /// Gets a human-readable description of the permissions.
    /// </summary>
    /// <param name="permissions">The participant's permissions.</param>
    /// <returns>A string describing the permissions.</returns>
    public static string GetDescription(this ParticipantPermissions permissions)
    {
        if (permissions == ParticipantPermissions.None)
            return "No access";

        var descriptions = new List<string>();

        if (permissions.CanAnswer())
            descriptions.Add("can answer");

        if (permissions.CanViewOwnResults())
            descriptions.Add("can view own results");

        if (permissions.CanViewOthersResults())
            descriptions.Add("can view others' results");

        return string.Join(", ", descriptions);
    }

    /// <summary>
    /// Creates permissions for common assignment scenarios.
    /// </summary>
    public static class CommonPermissions
    {
        /// <summary>
        /// Individual assignment: Participant answers and sees only their results.
        /// </summary>
        public static ParticipantPermissions Individual
            => ParticipantPermissions.CanAnswerAndViewOwn;

        /// <summary>
        /// Peer review: Participant answers and can see everyone's results.
        /// </summary>
        public static ParticipantPermissions PeerReview
            => ParticipantPermissions.CanAnswerAndViewAll;

        /// <summary>
        /// Observer: Can view all results but cannot answer.
        /// </summary>
        public static ParticipantPermissions Observer
            => ParticipantPermissions.ViewOnlyAll;

        /// <summary>
        /// Minimal: Can only answer, cannot see any results.
        /// </summary>
        public static ParticipantPermissions AnswerOnly
            => ParticipantPermissions.CanAnswer;

        /// <summary>
        /// Results reviewer: Can view own and others' results after answering.
        /// </summary>
        public static ParticipantPermissions ResultsReviewer
            => ParticipantPermissions.CanAnswerAndViewAll;
    }
}