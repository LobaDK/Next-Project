using Flexinets.Radius.Core;
using Flexinets.Radius;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Services;

public sealed class RadiusClientSingleton : IRadiusClientSingleton, IDisposable
{
    private readonly RadiusClient _radiusClient;
    private readonly IPEndPoint _serverEndpoint;
    private readonly string _secretKey;
    private readonly int _timeout;
    private readonly int _retries;

    public RadiusClientSingleton(string hostIp, int port, string secretKey, int timeout = 3, int retries = 2)
    {
        IPEndPoint clientListenEndpoint = new(IPAddress.Any, 0);

        IRadiusDictionary radiusPacketDictionary = RadiusDictionary.Parse(DefaultDictionary.RadiusDictionary);
        RadiusPacketParser radiusPacketParser = new(NullLogger<RadiusPacketParser>.Instance, radiusPacketDictionary, true);
        
        _radiusClient = new(clientListenEndpoint, radiusPacketParser);

        if (!IPAddress.TryParse(hostIp, out var ipAddress))
        {
            throw new ArgumentException($"Invalid IP address: '{hostIp}'", nameof(hostIp));
        }
        _serverEndpoint = new IPEndPoint(ipAddress, port);
        _secretKey = secretKey;
        _timeout = timeout;
        _retries = retries;
    }

    public async Task<IRadiusPacket> SendAsync(IRadiusPacket packet)
    {
        return await SendWithRetryAsync(packet);
    }

    public async Task<IRadiusPacket> AccessRequestAsync(string username, string password)
    {
        Dictionary<string, object> attributes = new()
        {
            { "User-Name", username },
            { "Acct-Session-Id", Guid.NewGuid().ToString() },
            { "NAS-Identifier", "NEXT Questionnaire" },
            { "Calling-Station-Id", 1115551212 },
            { "User-Password", password }
        };

        return await AccessRequestAsync(username, password, attributes);
    }

    public async Task<IRadiusPacket> AccessRequestAsync(string username, string password, Dictionary<string, object> attributes)
    {
        RadiusPacket packet = new(PacketCode.AccessRequest, 0, _secretKey);

        foreach (KeyValuePair<string, object> attr in attributes)
        {
            if (attr.Value is string stringValue)
            {
                packet.AddAttribute(attr.Key, stringValue);
            }
            else if (attr.Value is uint uIntValue)
            {
                packet.AddAttribute(attr.Key, uIntValue);
            }
            else if (attr.Value.GetType() == typeof(int))
            {
                packet.AddAttribute(attr.Key, (uint)(int)attr.Value);
            }
            else if (attr.Value is byte[] byteArrayValue)
            {
                packet.AddAttribute(attr.Key, byteArrayValue);
            }
            else if (attr.Value is IPAddress iPAddressValue)
            {
                packet.AddAttribute(attr.Key, iPAddressValue);
            }
            else
            {
                throw new ArgumentException($"Unsupported attribute type for RADIUS attribute '{attr.Key}' with value type '{attr.Value.GetType()}'");
            }
        }

        return await SendWithRetryAsync(packet);
    }

    public void Dispose()
    {
        _radiusClient.Dispose();
    }

    private async Task<IRadiusPacket> SendWithRetryAsync(IRadiusPacket packet)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                return await _radiusClient.SendPacketAsync(packet, _serverEndpoint);
            }
            catch (Exception ex) when (attempt < _retries)
            {
                attempt++;
                // Optionally log the exception here
                await Task.Delay(_timeout * 1000); // Wait before retrying
            }
        }
    }
}
