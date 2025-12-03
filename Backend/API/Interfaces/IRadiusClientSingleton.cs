using Flexinets.Radius.Core;

namespace API.Interfaces;

public interface IRadiusClientSingleton
{
    Task<IRadiusPacket> SendAsync(IRadiusPacket packet);
    Task<IRadiusPacket> AccessRequestAsync(string username, string password);
    Task<IRadiusPacket> AccessRequestAsync(string username, string password, Dictionary<string, object> attributes);
    
}
