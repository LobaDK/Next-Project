namespace QuestionnaireAPI.Interfaces;

/// <summary>
/// Provides methods and properties to monitor and control maintenance mode for the application.
/// </summary>
public interface IMaintenanceMonitor
{
    /// <summary>
    /// Gets a value indicating whether maintenance mode is enabled.
    /// </summary>
    bool IsMaintenanceEnabled { get; }
    /// <summary>
    /// Gets the reason for maintenance mode, if any.
    /// </summary>
    string Reason { get; }
    /// <summary>
    /// Enables maintenance mode.
    /// </summary>
    void EnableMaintenance();
    /// <summary>
    /// Enables maintenance mode with a specified reason.
    /// </summary>
    /// <param name="reason">The reason for enabling maintenance mode.</param>
    void EnableMaintenance(string reason);
    /// <summary>
    /// Disables maintenance mode and optionally clears the reason.
    /// </summary>
    /// <param name="clearReason">If true, clears the maintenance reason.</param>
    void DisableMaintenance(bool clearReason);
    /// <summary>
    /// Sets the reason for maintenance mode.
    /// </summary>
    /// <param name="reason">The reason for maintenance mode.</param>
    void SetMaintenanceReason(string reason);
    /// <summary>
    /// Clears the maintenance reason.
    /// </summary>
    void ClearMaintenanceReason();
}
