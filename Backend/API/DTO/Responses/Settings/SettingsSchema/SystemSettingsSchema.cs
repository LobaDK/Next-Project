namespace API.DTO.Responses.Settings.SettingsSchema;

public record class SystemSettingsSchema
{
    public required AdminUsername AdminUsername { get; set; }
    public required AdminPassword AdminPassword { get; set; }
    public required ListenIPSchema ListenIP { get; set; }
    public required HttpPortSchema HttpPort { get; set; }
    public required HttpsPortSchema HttpsPort { get; set; }
    public required UseSSLSchema UseSSL { get; set; }
    public required PfxCertificatePathSchema PfxCertificatePath { get; set; }
}

public record class AdminUsername : SettingsSchemaBase
{ }

public record class AdminPassword : SettingsSchemaBase
{ }

public record class ListenIPSchema : SettingsSchemaBase
{ }

public record class HttpPortSchema : SettingsSchemaBase
{ }

public record class HttpsPortSchema : SettingsSchemaBase
{ }

public record class UseSSLSchema : SettingsSchemaBase
{ }

public record class PfxCertificatePathSchema : SettingsSchemaBase
{ }