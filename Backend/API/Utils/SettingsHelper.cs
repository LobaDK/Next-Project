
namespace API.Utils;

/// <summary>
/// Helper class for managing application settings files, including creation of default settings
/// and checking for settings file existence.
/// </summary>
/// <param name="settingsFile">The file path to the settings file to be managed</param>
/// <param name="logger">The logger instance for logging messages</param>
public class SettingsHelper(string settingsFile, ILogger<SettingsHelper> logger)
{
    private readonly string _settingsFile = settingsFile;
    private readonly RootSettings _defaultSettings = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = JsonSerializerUtility.ConfigureJsonSerializerSettings();
    private readonly ILogger<SettingsHelper> _logger = logger;

    /// <summary>
    /// Determines whether the settings file exists on the file system.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the settings file exists; otherwise, <c>false</c>.
    /// </returns>
    public bool SettingsExists()
    {
        return File.Exists(_settingsFile);
    }

    /// <summary>
    /// Creates a default settings file by serializing the default settings to JSON and writing it to the configured file path.
    /// After creating the file, prompts the user to configure required settings before continuing.
    /// </summary>
    /// <remarks>
    /// This method will overwrite any existing settings file. It handles both normal console input and redirected input
    /// (such as when running in Visual Studio Code debug console) by using appropriate input methods.
    /// The method blocks execution until the user provides input, allowing them time to manually edit the generated settings file.
    /// </remarks>
    public void CreateDefault()
    {
        string json = JsonSerializer.Serialize(_defaultSettings, _jsonSerializerOptions);
        Write(_settingsFile, json);

        // Notify the user without blocking for input so automated runs are not halted.
        _logger.LogWarning(@"
        A default settings file has been created at '{settingsFile}'.
        Please review and update the settings as necessary before restarting the application.
        ", _settingsFile);
    }

    public void CheckSettingsVersion()
    {
        RootSettings currentSettings = JsonSerializer.Deserialize<RootSettings>(Read(_settingsFile), _jsonSerializerOptions) ?? throw new InvalidOperationException("Failed to deserialize settings file.");
        RootSettings defaultSettings = new();

        using JsonDocument doc = JsonDocument.Parse(Read(_settingsFile));
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty("Version", out JsonElement versionElement))
        {
            currentSettings.Version = 0;
        }

        if (currentSettings.Version == defaultSettings.Version)
        {
            return;
        }
        else if (currentSettings.Version > defaultSettings.Version)
        {
            _logger.LogWarning(@"
            The current settings file version ({currentVersion}) is newer than the application supports ({defaultVersion}).
            Please update the application to a newer version that supports this settings file.
            ", currentSettings.Version, defaultSettings.Version);
        }
        else
        {
            _logger.LogWarning(@"
            The current settings file version ({currentVersion}) is older than the application supports ({defaultVersion}).
            The settings file will be upgraded to the latest version.
            ", currentSettings.Version, defaultSettings.Version);

            Upgrade();
        }
    }

    public void Upgrade()
    {
        RootSettings currentSettings = JsonSerializer.Deserialize<RootSettings>(Read(_settingsFile), _jsonSerializerOptions) ?? throw new InvalidOperationException("Failed to deserialize settings file.");
        RootSettings defaultSettings = new();

        Upgrade(currentSettings, defaultSettings);
    }
    
    public void Upgrade(RootSettings currentSettings, RootSettings defaultSettings)
    {
        currentSettings.MergeWith(defaultSettings);
        currentSettings.Version = defaultSettings.Version;

        string json = JsonSerializer.Serialize(currentSettings, _jsonSerializerOptions);
        Write(_settingsFile, json);
    }

    private static void Write(string path, string data)
    {
        File.WriteAllText(path, data);
    }

    private static string Read(string path)
    {
        return File.ReadAllText(path);
    }
}
