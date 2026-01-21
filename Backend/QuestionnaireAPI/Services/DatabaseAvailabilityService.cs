using QuestionnaireAPI.Interfaces;

namespace QuestionnaireAPI.Services;

public class DatabaseAvailabilityService(ILogger<DatabaseAvailabilityService> logger) : IDatabaseAvailabilityService
{
    private ILogger<DatabaseAvailabilityService> _logger = logger;
    private bool _isDatabaseAvailable = true;
    private readonly Lock _lock = new();

    public bool IsDatabaseAvailable
    {
        get
        {
            lock (_lock)
            {
                return _isDatabaseAvailable;
            }
        }
    }

    public bool CheckDatabaseHealth()
    {
        lock (_lock)
        {
            return _isDatabaseAvailable;
        }
    }

    public void SetDatabaseAvailable(bool available)
    {
        lock (_lock)
        {
            _logger.LogInformation("Setting database availability to {Available}", available);
            _isDatabaseAvailable = available;
        }
    }
}
