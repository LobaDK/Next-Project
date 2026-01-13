namespace API.Services.Authentication;

using System.Text.RegularExpressions;

/// <summary>
/// Active Directory authentication bridge implementation using LDAP.
/// </summary>
public partial class ActiveDirectoryAuthenticationBridge : BaseAuthenticationBridge
{
    private readonly ILogger<ActiveDirectoryAuthenticationBridge> _Logger;
    private readonly LDAPSettings _LdapSettings;
    private readonly IWebHostEnvironment _environment;
    private LdapConnection? _Connection;
    private static readonly Regex LdapErrorCodeRegex = GetLdapErrorCodeRegex();

    public ActiveDirectoryAuthenticationBridge(
        ILogger<ActiveDirectoryAuthenticationBridge> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment) 
        : base(new LDAPFieldMappingProvider())
    {
        _Logger = logger;
        _LdapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
        _environment = environment;
    }

    public override void Authenticate(string username, string password)
    {
        _Logger.LogInformation("Starting LDAP authentication for user: {Username}", username);

        int port;
        LdapConnectionOptions connectionOptions = new();
        if (_LdapSettings.UseSSL)
        {
            connectionOptions.UseSsl();
            port = _LdapSettings.SSLPort;
        }
        else
        {
            port = _LdapSettings.Port;
        }

        try
        {
            if (_Connection is not null && _Connection.Connected)
            {
                _Logger.LogDebug("Disconnecting existing LDAP connection");
                _Connection.Disconnect();
            }

            _Logger.LogDebug("Creating new LDAP connection to {Host}:{Port}", _LdapSettings.Host, port);
            _Connection = CreateConnection(connectionOptions);
            LdapSearchConstraints constraints = new();
            constraints.ReferralFollowing = true;
            _Connection.Constraints = constraints;

            _Connection.Connect(_LdapSettings.Host, port);

            BindWithSimple(username, password);
            _Logger.LogInformation("LDAP authentication successful for user: {Username}", username);

        }
        catch (LdapException ex)
        {
            // https://ldap.com/ldap-result-code-reference/
            // Invalid credentials exception data:
            // Incorrect username/password:             80090308: LdapErr: DSID-0C090434, comment: AcceptSecurityContext error, data 52e, v4f7c\0
            // Account disabled:                        80090308: LdapErr: DSID-0C090434, comment: AcceptSecurityContext error, data 533, v4f7c\0
            // User must change password at next logon: 80090308: LdapErr: DSID-0C090434, comment: AcceptSecurityContext error, data 773, v4f7c\0
            // Expired account:                         80090308: LdapErr: DSID-0C090434, comment: AcceptSecurityContext error, data 701, v4f7c\0
            // Account lockout:                         80090308: LdapErr: DSID-0C090434, comment: AcceptSecurityContext error, data 775, v4f7c\0

            string errorMessage;
            if (ex.ResultCode == LdapException.InvalidCredentials)
            {
                Match dataMatch = LdapErrorCodeRegex.Match(ex.LdapErrorMessage);
                
                LdapAuthenticationErrorReasons reason;
                if (_environment.IsDevelopment())
                {
                    (errorMessage, reason) = (
                        dataMatch.Success ? dataMatch.Groups[1].Value switch
                        {
                            "52e" => ("Incorrect username or password.", LdapAuthenticationErrorReasons.InvalidCredentials),
                            "533" => ("Account is disabled.", LdapAuthenticationErrorReasons.AccountDisabled),
                            "701" => ("Account has expired.", LdapAuthenticationErrorReasons.AccountExpired),
                            "773" => ("User must change password at next logon.", LdapAuthenticationErrorReasons.PasswordHasExpired),
                            "775" => ("Account is locked out due to multiple failed login attempts.", LdapAuthenticationErrorReasons.AccountIsLockedOut),
                            _ => ("Invalid credentials provided.", LdapAuthenticationErrorReasons.InvalidCredentials)
                        } : ("Invalid credentials provided.", LdapAuthenticationErrorReasons.InvalidCredentials)
                    );    
                }
                else
                {
                    (errorMessage, reason) = (
                        dataMatch.Success ? dataMatch.Groups[1].Value switch
                        {
                            "52e" => ("Invalid credentials provided.", LdapAuthenticationErrorReasons.InvalidCredentials),
                            _ => ("Authentication failed due to account issues.", LdapAuthenticationErrorReasons.AccountLoginError)
                        } : ("Invalid credentials provided.", LdapAuthenticationErrorReasons.InvalidCredentials)
                    );
                }

                _Logger.LogWarning("LDAP authentication failed for user: {Username} - {ErrorMessage}", username, errorMessage);
                throw new LDAPException.LdapAuthenticationErrorException(reason, errorMessage, ex);   
            }
            else
            {
                errorMessage = ex.ResultCode switch
                {
                    LdapException.ConnectError => "Unable to connect to the authentication server.",
                    LdapException.LdapTimeout => "The authentication server did not respond in a timely manner.",
                    LdapException.ServerDown => "The authentication server is currently unreachable.",
                    _ => $"An unexpected error occurred during authentication. Please try again later. (Error Code: {ex.ResultCode})"
                };

                _Logger.LogError(ex, "LDAP authentication error for user: {Username} - {ErrorMessage}", username, errorMessage);
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }

    public override TUser? SearchUser<TUser>(string username) where TUser : default
    {
        _Logger.LogDebug("Starting user search for: {Username}", username);
        
        string domain = _LdapSettings.FQDN.Split(".", 2).Last();

        // NETBIOS\\username
        string sAMAccountName = $"{string.Join("", domain.Split('.'))}\\\\{username}";
        // username@domain
        string userPrincipalName = $"{sAMAccountName.Split('\\').Last()}@{domain}";

        string searchFilter = $"(|(userPrincipalName={userPrincipalName})(sAMAccountName={sAMAccountName}))";
        _Logger.LogDebug("Using search filter: {SearchFilter} with BaseDN: {BaseDN}", searchFilter, _LdapSettings.BaseDN);

        var users = SearchLDAP<TUser>(searchFilter, _LdapSettings.BaseDN, LdapConnection.ScopeSub);
        if (users.Count == 0)
        {
            _Logger.LogWarning("No users found for username: {Username}", username);
            return default;
        }
        
        _Logger.LogDebug("Found {UserCount} user(s) for username: {Username}", users.Count, username);
        TUser userSearch = users.First();

        return userSearch;
    }

    public override TGroup? SearchGroup<TGroup>(string groupName) where TGroup : default
    {
        _Logger.LogDebug("Starting group search for: {GroupName}", groupName);
        
        string escapedGroupName = EscapeLDAPSearchFilter(groupName);
        
        string searchFilter = $"(&(objectCategory=group)(cn={escapedGroupName}))";
        _Logger.LogDebug("Using group search filter: {SearchFilter}", searchFilter);

        var groups = SearchLDAP<TGroup>(searchFilter, _LdapSettings.BaseDN);
        if (groups.Count == 0)
        {
            _Logger.LogWarning("No groups found for group name: {GroupName}", groupName);
            return default;
        }

        _Logger.LogDebug("Found {GroupCount} group(s) for group name: {GroupName}", groups.Count, groupName);
        TGroup ldapGroup = groups.First();

        return ldapGroup;
    }

    public override TEntity? SearchId<TEntity>(string id) where TEntity : default
    {
        EnsureBoundConnection();

        _Logger.LogDebug("Starting entity search by ID: {Id}", id);
        
        if (string.IsNullOrEmpty(id))
        {
            _Logger.LogWarning("Provided ID is null or empty");
            return default;
        }

        if (!Guid.TryParse(id, out Guid guid))
        {
            _Logger.LogError("The provided Id is not a valid GUID: {Id}", id);
            throw new ArgumentException("The provided Id is not a valid GUID.");
        }

        string searchFilter = $"(objectGUID={EscapeGUID(guid)})";
        _Logger.LogDebug("Using ID search filter: {SearchFilter}", searchFilter);
        
        var results = SearchLDAP<TEntity>(searchFilter, _LdapSettings.BaseDN);
        if (results.Count == 0)
        {
            _Logger.LogWarning("No entities found for ID: {Id}", id);
            return default;
        }
        
        _Logger.LogDebug("Found {ResultCount} entity(ies) for ID: {Id}", results.Count, id);
        TEntity ldapObject = results.First();

        return ldapObject;
    }

    public override (List<TMockUser>, string, bool) SearchUserPagination<TMockUser>(string username, string? userRole, int pageSize, string? sessionId) where TMockUser : default
    {
        _Logger.LogWarning("SearchUserPagination not implemented yet.");
        throw new NotImplementedException("SearchUserPagination not yet implemented.");
    }

    public override void Dispose()
    {
        if (_Connection is not null)
        {
            try
            {
                if (_Connection.Connected)
                {
                    _Connection.Disconnect();
                }
                _Connection.Dispose();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error disposing LDAP connection");
            }
            finally
            {
                _Connection = null;
            }
        }
    }

    public override bool IsConnected() => _Connection?.Connected ?? false;

    /// <summary>
    /// Searches the LDAP directory using the specified search query and maps the results to a list of objects of type <typeparamref name="TLdapResult"/>.
    /// </summary>
    /// <param name="searchQuery">The LDAP search query.</param>
    /// <param name="baseDN">The base distinguished name (DN) to search from.</param>
    /// <param name="scope">The scope of the search. Defaults to <see cref="LdapConnection.ScopeSub"/>.</param>
    /// <returns>A list of objects of type <typeparamref name="TLdapResult"/> mapped from the LDAP search results.</returns>
    private List<TLdapResult> SearchLDAP<TLdapResult>(string searchQuery, string baseDN, int scope = LdapConnection.ScopeSub) where TLdapResult : new()
    {
        _Logger.LogDebug("Executing LDAP search - Query: {SearchQuery}, BaseDN: {BaseDN}, Scope: {Scope}", searchQuery, baseDN, scope);
        
        EnsureBoundConnection();

        string[] attributes = GetEntriesToQuery<TLdapResult>();
        _Logger.LogDebug("Requesting attributes: {Attributes}", string.Join(", ", attributes));

        ILdapSearchResults searchResults = _Connection!.Search(
            baseDN,
            scope,
            searchQuery,
            attributes,
            false
        );

        List<TLdapResult> mappedLdapResults = [];

        while (searchResults.HasMore())
        {
            LdapEntry entry = searchResults.Next();
            mappedLdapResults.Add(MapToModel<TLdapResult>(ConvertLdapEntryToDictionary(entry)));
        }

        _Logger.LogDebug("LDAP search completed - Found {ResultCount} entries", mappedLdapResults.Count);
        return mappedLdapResults;
    }

    /// <summary>
    /// Ensures that the LDAP connection is established and bound.
    /// </summary>
    private void EnsureBoundConnection()
    {
        if (_Connection is null || !_Connection.Bound)
        {
            _Logger.LogDebug("Connection is null or not bound, establishing service account connection");
            Authenticate(_LdapSettings.SA, _LdapSettings.SAPassword);
        }
    }

    /// <summary>
    /// Escapes special characters in an LDAP search filter.
    /// </summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value.</returns>
    public static string EscapeLDAPSearchFilter(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        // RFC4515: (, ), *, \, and NUL must be escaped as \28, \29, \2a, \5c, \00 respectively
        var escaped = new System.Text.StringBuilder();
        foreach (char c in value)
        {
            switch (c)
            {
                case '\\':
                    escaped.Append(@"\5c");
                    break;
                case '*':
                    escaped.Append(@"\2a");
                    break;
                case '(':
                    escaped.Append(@"\28");
                    break;
                case ')':
                    escaped.Append(@"\29");
                    break;
                case '\0':
                    escaped.Append(@"\00");
                    break;
                default:
                    escaped.Append(c);
                    break;
            }
        }
        return escaped.ToString();
    }

    /// <summary>
    /// Escapes a GUID for use in an LDAP search filter.
    /// </summary>
    /// <param name="guid">The GUID to escape.</param>
    /// <returns>The escaped GUID as a string.</returns>
    public static string EscapeGUID(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        return string.Concat(bytes.Select(b => $"\\{b:X2}"));
    }

    /// <summary>
    /// Converts an LDAP entry to a dictionary.
    /// </summary>
    private static Dictionary<string, object> ConvertLdapEntryToDictionary(LdapEntry entry)
    {
        var dict = new Dictionary<string, object>();
        LdapAttributeSet attributeSet = entry.GetAttributeSet();
        foreach (LdapAttribute attribute in attributeSet)
        {
            dict[attribute.Name] = attribute;
        }
        return dict;
    }

    /// <summary>
    /// Authenticates a user against the LDAP server using simple bind with the provided username and password.
    /// The username is converted to a User Principal Name (UPN) format by appending the domain extracted from the LDAP settings.
    /// </summary>
    /// <param name="username">The username of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    private void BindWithSimple(string username, string password)
    {
        _Logger.LogDebug("Attempting simple bind for user: {Username}", username);
        
        string[] fqdnParts = _LdapSettings.FQDN.Split('.');
        if (fqdnParts.Length < 2)
        {
            _Logger.LogError("Invalid FQDN format: {FQDN}", _LdapSettings.FQDN);
            throw new InvalidOperationException("FQDN must contain at least one dot and two segments.");
        }
        string domain = string.Join('.', fqdnParts.Skip(1));
        username = $"{username}@{domain}";
        _Logger.LogDebug("Using UPN format for authentication: {UPN}", username);

        // dn = The DistinguishedName of the object (user) to authenticate as.
        // UPN (User Principal Name) (username@domain),
        // and sAMAccountName (NETBIOS\\username) also works.
        _Connection = GetConnection();
        
        try
        {
            _Connection.Bind(username, password);
            _Logger.LogDebug("Simple bind successful for user: {Username}", username);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Simple bind failed for user: {Username}", username);
            throw;
        }
    }

    /// <summary>
    /// Gets an existing LDAP connection or creates a new one if none exists.
    /// </summary>
    /// <param name="options">Optional connection options to configure the LDAP connection. If null, default options will be used.</param>
    /// <returns>An <see cref="LdapConnection"/> instance that can be used for LDAP operations.</returns>
    private LdapConnection GetConnection(LdapConnectionOptions? options = null)
    {
        if (_Connection is null)
        {
            _Logger.LogDebug("Creating new LDAP connection");
        }
        return _Connection ?? CreateConnection(options);
    }

    /// <summary>
    /// Creates a new LDAP connection instance with optional configuration.
    /// </summary>
    /// <param name="options">Optional LDAP connection configuration. If null, creates a connection with default settings.</param>
    /// <returns>A new <see cref="LdapConnection"/> instance configured with the specified options or default settings.</returns>
    private static LdapConnection CreateConnection(LdapConnectionOptions? options = null)
    {
        return options is null ? new LdapConnection() : new LdapConnection(options);
    }

    [GeneratedRegex(@"data\s([0-9a-fA-F]+)", RegexOptions.Compiled)]
    private static partial Regex GetLdapErrorCodeRegex();
}