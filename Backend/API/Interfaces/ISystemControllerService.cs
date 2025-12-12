namespace API.Interfaces;

public interface ISystemControllerService
{
    bool StopServer();
    FileResult ExportSettings();
    Task<bool> ImportSettings(IFormFile file);
    Task<FileResult> GetLogFile(string filename);
    List<string> GetLogFileNames();
    SettingsFetchResponse GetSettings();
    SettingsSchema GetSettingsSchema();
    Task<bool> UpdateSettings(UpdateSettingsRequest rootSettings);
    Task<bool> PatchSettings(PatchSettingsRequest rootSettings);
}