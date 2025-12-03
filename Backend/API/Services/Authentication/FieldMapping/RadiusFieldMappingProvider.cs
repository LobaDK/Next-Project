namespace API.Services.Authentication.FieldMapping;

/// <summary>
/// Maps RADIUS attributes to application user fields.
/// </summary>
public class RadiusFieldMappingProvider : IFieldMappingProvider
{
    private static readonly Dictionary<Type, Dictionary<string, string>> _mappings = new()
    {
        [typeof(BasicUserInfo)] = new()
        {
            [nameof(BasicUserInfo.Name)] = "User-Name",
            [nameof(BasicUserInfo.Username)] = "User-Name",
            [nameof(BasicUserInfo.MemberOf)] = "Group-Name"
        },
        [typeof(BasicUserInfoWithUserID)] = new()
        {
            [nameof(BasicUserInfoWithUserID.UserId)] = "Calling-Station-Id",
            [nameof(BasicUserInfoWithUserID.Name)] = "User-Name",
            [nameof(BasicUserInfoWithUserID.Username)] = "User-Name",
            [nameof(BasicUserInfoWithUserID.MemberOf)] = "Group-Name"
        },
        [typeof(BasicGroupInfo)] = new()
        {
            [nameof(BasicGroupInfo.GroupName)] = "Group-Name"
        }
    };

    private static readonly Dictionary<string, Func<object, Type, object>> _converters = new()
    {
        ["Calling-Station-Id"] = (obj, type) =>
        {
            if (obj is string callingStationId)
            {
                if (type == typeof(Guid))
                {
                    return Guid.Parse(callingStationId);
                }
                else if (type == typeof(string))
                {
                    return callingStationId;
                }
            }
            throw new InvalidCastException("Expected string for Calling-Station-Id conversion.");
        },
        ["User-Name"] = (obj, type) =>
        {
            if (obj is string userName)
            {
                return userName;
            }
            throw new InvalidCastException("Expected string for User-Name conversion.");
        },
        ["Group-Name"] = (obj, type) =>
        {
            if (obj is string groupName)
            {
                return groupName;
            }
            throw new InvalidCastException("Expected string for Group-Name conversion.");
        }
    };

    public Dictionary<string, string> GetFieldMappings<TModel>()
    {
        return _mappings.TryGetValue(typeof(TModel), out var mappings)
            ? mappings : [];
    }

    public Dictionary<string, Func<object, Type, object>> GetFieldConverter()
    {
        return _converters;
    }
}