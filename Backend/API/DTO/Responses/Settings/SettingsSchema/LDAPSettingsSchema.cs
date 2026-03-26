namespace API.DTO.Responses.Settings.SettingsSchema;

public record class LDAPSettingsSchema
{
    public required HostSchema Host { get; set; }
    public required PortSchema Port { get; set; }
    public required SSLPortSchema SSLPort { get; set; }
    public required FQDNSchema FQDN { get; set; }
    public required BaseDNSchema BaseDN { get; set; }
    public required SASchema SA { get; set; }
    public required SAPasswordSchema SAPassword { get; set; }
    public required RoleMappingsCNSchema RoleMappingsCN { get; set; }
}

public record class HostSchema : SettingsSchemaBase
{ }

public record class PortSchema : SettingsSchemaExtended
{ }

public record class SSLPortSchema : SettingsSchemaExtended
{ }

public record class FQDNSchema : SettingsSchemaBase
{ }

public record class BaseDNSchema : SettingsSchemaBase
{ }

public record class SASchema : SettingsSchemaBase
{ }

public record class SAPasswordSchema : SettingsSchemaBase
{ }

public record class RoleMappingsCNSchema : SettingsSchemaExtended
{ }