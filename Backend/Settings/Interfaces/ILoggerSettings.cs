
namespace Settings.Interfaces;

public interface ILoggerSettings<TDB>
{
    public Dictionary<string, LogLevel> LogLevel { get; set; }
    public TDB DBLogger { get; set; }
}
