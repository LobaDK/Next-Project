using API.Services.Authentication.Exceptions;
using API.Services.Authentication.FieldMapping;
using Flexinets.Radius;
using Flexinets.Radius.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Settings.Interfaces;

namespace API.Services.Authentication;

/// <summary>
/// RADIUS authentication implementation that authenticates users against a RADIUS/NPS server.
/// </summary>
public class RadiusAuthenticationBridge : BaseAuthenticationBridge
{
    private readonly ILogger<RadiusAuthenticationBridge> _logger;
    private readonly IHostEnvironment _environment;
    private readonly RadiusClientSingleton _radiusClient;
    private bool _connected = false;

    public RadiusAuthenticationBridge(
        ILogger<RadiusAuthenticationBridge> logger,
        IConfiguration configuration,
        IHostEnvironment environment,
        RadiusClientSingleton radiusClient)
        : base(new RadiusFieldMappingProvider())
    {
        _logger = logger;
        _environment = environment;
        _radiusClient = radiusClient;
    }

    public override async void Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new RadiusAuthenticationException(
                RadiusAuthenticationErrorReasons.InvalidCredentials,
                "Username and password are required.");
        }

        IRadiusPacket response = await _radiusClient.AccessRequestAsync(username, password);

        PacketCode responseCode = response.Code;

        if (responseCode == PacketCode.AccessAccept)
        {
            _connected = true;
            return;
        }
    }

    public override TUser? SearchUser<TUser>(string username) where TUser : default
    {
        throw new NotImplementedException();
    }

    public override (List<TUser>, string, bool) SearchUserPagination<TUser>(string username, string? userRole, int pageSize, string? sessionId)
    {
        throw new NotImplementedException();
    }

    public override TEntity? SearchId<TEntity>(string Id) where TEntity : default
    {
        throw new NotImplementedException();
    }

    public override TGroup? SearchGroup<TGroup>(string groupName) where TGroup : default
    {
        throw new NotImplementedException();
    }

    public override bool IsConnected()
    {
        return _connected;
    }

    private static string GetErrorMessage(PacketCode code, bool isDevelopment)
    {
        if (isDevelopment)
        {
            return code switch
            {
                PacketCode.AccessReject => "Access rejected by RADIUS server - invalid credentials or user not authorized.",
                PacketCode.AccessChallenge => "Additional authentication required (e.g., MFA token).",
                _ => $"RADIUS server returned unexpected response: {code}"
            };
        }

        return code switch
        {
            PacketCode.AccessReject => "Invalid credentials provided.",
            PacketCode.AccessChallenge => "Additional authentication required.",
            _ => "Authentication service error."
        };
    }

    public override void Dispose()
    {
        _radiusClient.Dispose();
    }
}

/// <summary>
/// Result of RADIUS authentication attempt.
/// </summary>
internal class RadiusAuthenticationResult
{
    public bool IsAuthenticated { get; init; }
    public PacketCode ResponseCode { get; init; }
    public Dictionary<string, object>? Attributes { get; init; }
}