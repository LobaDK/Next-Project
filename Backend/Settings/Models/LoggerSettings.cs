
namespace Settings.Models;

public class LoggerSettings : Base, ILoggerSettings<DBLoggerSettings>
{
    [JsonIgnore]
    public override string Key { get; } = "Logging";
    
    [Description("Defines the default global log levels for the application.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Error },
        { "Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning }
    };
    public DBLoggerSettings DBLogger { get; set; } = new DBLoggerSettings();
}
