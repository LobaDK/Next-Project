using QuestionnaireAPI.Interfaces;

namespace QuestionnaireAPI.Services;

public class MaintenanceMonitor(ILogger<MaintenanceMonitor> logger) : IMaintenanceMonitor
{
    private readonly ILogger<MaintenanceMonitor> _logger = logger;
    private bool _isMaintenanceEnabled = false;
    private string _reason = string.Empty;
    private readonly Lock _lock = new();

    public bool IsMaintenanceEnabled
    {
        get
        {
            lock (_lock)
            {
                return _isMaintenanceEnabled;
            }
        }
    }

    public string Reason
    {
        get
        {
            lock (_lock)
            {
                return _reason;
            }
        }
    }

    public void EnableMaintenance()
    {
        lock (_lock)
        {
            if (!_isMaintenanceEnabled)
            {
                _logger.LogDebug("Enabling maintenance mode");
                _isMaintenanceEnabled = true;
            }
        }
    }

    public void EnableMaintenance(string reason)
    {
        lock (_lock)
        {
            if (!_isMaintenanceEnabled)
            {
                _logger.LogDebug("Enabling maintenance mode with reason: {Reason}", reason);
                _isMaintenanceEnabled = true;
            }
            _reason = reason;
        }
    }

    public void DisableMaintenance()
    {
        lock (_lock)
        {
            if (_isMaintenanceEnabled)
            {
                _logger.LogDebug("Disabling maintenance mode");
                _isMaintenanceEnabled = false;
            }
        }
    }

    public void DisableMaintenance(bool clearReason = false)
    {
        lock (_lock)
        {
            if (_isMaintenanceEnabled)
            {
                _logger.LogDebug("Disabling maintenance mode");
                _isMaintenanceEnabled = false;
            }
            if (clearReason && !string.IsNullOrEmpty(_reason))
            {
                _logger.LogDebug("Clearing maintenance reason");
                _reason = string.Empty;
            }
        }
    }

    public void SetMaintenanceReason(string reason)
    {
        lock (_lock)
        {
            if (string.IsNullOrEmpty(_reason))
            {
                _logger.LogDebug("Setting maintenance reason to {Reason}", reason);
            }
            else
            {
                _logger.LogDebug("Updating maintenance reason from {OldReason} to {NewReason}", _reason, reason);
            }
            _reason = reason;
        }
    }

    public void ClearMaintenanceReason()
    {
        lock (_lock)
        {
            if (!string.IsNullOrEmpty(_reason))
            {
                _logger.LogDebug("Clearing maintenance reason");
                _reason = string.Empty;
            }
        }
    }
}
