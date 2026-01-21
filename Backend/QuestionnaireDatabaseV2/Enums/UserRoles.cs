using System.Text.Json.Serialization;

namespace QuestionnaireDatabaseV2.Enums;

/// <summary>
/// Represents the role of a user in the system.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    /// <summary>
    /// Default user with standard permissions.
    /// </summary>
    DefaultUser,

    /// <summary>
    /// Student role with permissions tailored for students.
    /// </summary>
    Student,

    /// <summary>
    /// Teacher role with permissions tailored for teachers.
    /// </summary>
    Teacher,

    /// <summary>
    /// Manager with elevated permissions for managing assignments and viewing aggregated data.
    /// </summary>
    Manager
}
