namespace QuestionnaireAPI.Interfaces;

public interface IDatabaseAvailabilityService
{
    bool IsDatabaseAvailable { get; }
    bool CheckDatabaseHealth();
    void SetDatabaseAvailable(bool available);
}
