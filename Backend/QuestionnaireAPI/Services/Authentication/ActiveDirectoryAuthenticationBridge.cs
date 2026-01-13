namespace API.Services.Authentication;

/// <summary>
/// Placeholder implementation of Active Directory authentication bridge.
/// This will be implemented when proper LDAP integration is needed.
/// </summary>
public class ActiveDirectoryAuthenticationBridge : BaseAuthenticationBridge
{
    private readonly ILogger<ActiveDirectoryAuthenticationBridge> _Logger;

    public ActiveDirectoryAuthenticationBridge(ILogger<ActiveDirectoryAuthenticationBridge> logger) 
        : base(new MockFieldMappingProvider())
    {
        _Logger = logger;
    }

    public override void Authenticate(string username, string password)
    {
        _Logger.LogWarning("ActiveDirectoryAuthenticationBridge not implemented yet. Use USEMOCKAUTH.txt file to enable mock authentication for development.");
        throw new NotImplementedException("Active Directory authentication not yet implemented. Use mock authentication for development.");
    }

    public override TUser? SearchUser<TUser>(string username) where TUser : default
    {
        _Logger.LogWarning("ActiveDirectoryAuthenticationBridge not implemented yet.");
        throw new NotImplementedException("Active Directory authentication not yet implemented. Use mock authentication for development.");
    }

    public override TGroup? SearchGroup<TGroup>(string groupName) where TGroup : default
    {
        _Logger.LogWarning("ActiveDirectoryAuthenticationBridge not implemented yet.");
        throw new NotImplementedException("Active Directory authentication not yet implemented. Use mock authentication for development.");
    }

    public override TEntity? SearchId<TEntity>(string id) where TEntity : default
    {
        _Logger.LogWarning("ActiveDirectoryAuthenticationBridge not implemented yet.");
        throw new NotImplementedException("Active Directory authentication not yet implemented. Use mock authentication for development.");
    }

    public override (List<TMockUser>, string, bool) SearchUserPagination<TMockUser>(string username, string? userRole, int pageSize, string? sessionId) where TMockUser : default
    {
        _Logger.LogWarning("ActiveDirectoryAuthenticationBridge not implemented yet.");
        throw new NotImplementedException("Active Directory authentication not yet implemented. Use mock authentication for development.");
    }

    public override void Dispose()
    {
        // Nothing to dispose
    }

    public override bool IsConnected() => false;
}