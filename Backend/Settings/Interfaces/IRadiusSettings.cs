namespace Settings.Interfaces;

public interface IRadiusSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string SecretKey { get; set; }
    public int Timeout { get; set; }
    public int Retries { get; set; }
}