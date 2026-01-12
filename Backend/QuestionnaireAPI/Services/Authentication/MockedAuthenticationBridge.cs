namespace API.Services.Authentication;

/// <summary>
/// Provides a mocked implementation of the authentication bridge for testing and development purposes.
/// This class simulates authentication using a local JSON file containing user data.
/// </summary>
public class MockedAuthenticationBridge(CacheService cacheService) : BaseAuthenticationBridge(new MockFieldMappingProvider())
{
    private readonly CacheService _CacheService = cacheService;
    private bool _Authenticated;
    private readonly List<MockedUser> _MockedUsers = JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText("./mocked_user_data.json")) ?? throw new InvalidOperationException("Mocked user data file is missing or invalid.");

    public override void Authenticate(string username, string password)
    {
        if (UsernameIsRole(username))
        {
            username = ConvertRoleToUsername(username);
        }

        if (_MockedUsers.Count != 0 && _MockedUsers.Any(u => u.Username == username && u.Password == password))
        {
            _Authenticated = true;
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
    }

    public override TUser? SearchUser<TUser>(string username) where TUser : default
    {
        EnsureAuthentication();

        if (UsernameIsRole(username))
        {
            username = ConvertRoleToUsername(username);
        }

        MockedUser? mockedUser = _MockedUsers.SingleOrDefault(u => u.Username == username);

        if (mockedUser == null)
        {
            return default;
        }

        var user = MapToModel<TUser>(ConvertMockEntryToDictionary(mockedUser));
        return user;
    }

    public override TGroup? SearchGroup<TGroup>(string groupName) where TGroup : default
    {
        EnsureAuthentication();

        string? group = _MockedUsers.Where(u => u.Role.ToString().Contains(groupName, StringComparison.OrdinalIgnoreCase)).Select(u => u.Role.ToString()).FirstOrDefault();

        if (group == null)
        {
            return default;
        }

        var groupObj = MapToModel<TGroup>(new Dictionary<string, object>
        {
            { "Role", group }
        });

        return groupObj;
    }

    public override TEntity? SearchId<TEntity>(string id) where TEntity : default
    {
        EnsureAuthentication();

        MockedUser? mockedUser = _MockedUsers.SingleOrDefault(u => u.Id == Guid.Parse(id));

        if (mockedUser == null)
        {
            return default;
        }

        var user = MapToModel<TEntity>(ConvertMockEntryToDictionary(mockedUser));
        return user;
    }

    public override (List<TMockUser>, string, bool) SearchUserPagination<TMockUser>(string username, string? userRole, int pageSize, string? sessionId) where TMockUser : default
    {
        EnsureAuthentication();

        IEnumerable<MockedUser> filteredUsers = _MockedUsers.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(userRole))
        {
            filteredUsers = filteredUsers.Where(u => u.Role.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase));
        }

        List<MockedUser> filteredUserList = [.. filteredUsers];

        MockSessionData sessionData;
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionData = new MockSessionData
            {
                FilteredUsers = filteredUserList,
                CurrentIndex = 0
            };
            sessionId = Guid.NewGuid().ToString();
            _CacheService.Set(sessionId, sessionData);
        }
        else
        {
            sessionData = _CacheService.Get<MockSessionData>(sessionId) ?? new MockSessionData
            {
                FilteredUsers = filteredUserList,
                CurrentIndex = 0
            };
        }

        List<TMockUser> pagedUsers = [];

        for (int i = 0; i < pageSize; i++)
        {
            if (sessionData.CurrentIndex + i >= sessionData.FilteredUsers.Count)
            {
                break;
            }

            MockedUser currentUser = sessionData.FilteredUsers[sessionData.CurrentIndex + i];
            TMockUser mappedUser = MapToModel<TMockUser>(ConvertMockEntryToDictionary(currentUser));
            pagedUsers.Add(mappedUser);
        }

        sessionData.CurrentIndex += pagedUsers.Count;
        bool hasMore = sessionData.CurrentIndex < sessionData.FilteredUsers.Count;

        _CacheService.Set(sessionId, sessionData);

        return (pagedUsers, sessionId, hasMore);
    }

    public override void Dispose()
    {
        // No resources to dispose
    }

    public override bool IsConnected() => _Authenticated;

    private static Dictionary<string, object> ConvertMockEntryToDictionary(MockedUser entry)
    {
        var dict = new Dictionary<string, object>();
        foreach (PropertyInfo prop in typeof(MockedUser).GetProperties())
        {
            dict[prop.Name] = prop.GetValue(entry) ?? "";
        }
        return dict;
    }

    private void EnsureAuthentication()
    {
        if (!_Authenticated)
        {
            // No need for actual credentials in mocked authentication
            _Authenticated = true;
        }
    }

    private string ConvertRoleToUsername(string role)
    {
        UserRoles? userRole = role.ToLower() switch
        {
            var r when r.StartsWith(UserRoles.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase) => UserRoles.Manager,
            var r when r.StartsWith(UserRoles.DefaultUser.ToString(), StringComparison.CurrentCultureIgnoreCase) => UserRoles.DefaultUser,
            _ => null
        };

        if (userRole is not null)
        {
            IEnumerable<MockedUser> query = _MockedUsers.Where(u => u.Role == userRole);
            if (UsernameEndsWithNumber(role, out int number))
            {
                // manager and manager1 should return the first manager user, manager2 the second, and so on
                query = query.Skip(number - 1);
            }
            return query.Select(u => u.Username).FirstOrDefault() ?? throw new InvalidOperationException("No user found for the specified role.");
        }

        throw new InvalidOperationException("Invalid role specified.");
    }

    private static bool UsernameIsRole(string username)
    {
        return username.Contains(UserRoles.Manager.ToString(), StringComparison.InvariantCultureIgnoreCase) ||
               username.Contains(UserRoles.DefaultUser.ToString(), StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool UsernameEndsWithNumber(string username, out int number)
    {
        number = 0;
        for (int i = username.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(username[i]))
            {
                number += (username[i] - '0') * (int)Math.Pow(10, username.Length - 1 - i);
            }
            else
            {
                break;
            }
        }
        return number > 0;
    }
}