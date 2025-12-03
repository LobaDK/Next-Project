namespace Settings.Models;

public class RadiusSettings : Base, IRadiusSettings
{
    [JsonIgnore]
    public override string Key { get; } = "RADIUS";

    [Description("The RADIUS server host address. Can be an IP address or domain name.")]
    public string Host { get; set; } = string.Empty;

    [Description("The RADIUS server port number.")]
    public int Port { get; set; } = 1812;

    [Description("The shared secret key for RADIUS authentication.")]
    public string SecretKey { get; set; } = string.Empty;

    [Description("The timeout value in seconds for RADIUS requests.")]
    public int Timeout { get; set; } = 5;

    [Description("The number of retries for failed RADIUS requests.")]
    public int Retries { get; set; } = 3;
}