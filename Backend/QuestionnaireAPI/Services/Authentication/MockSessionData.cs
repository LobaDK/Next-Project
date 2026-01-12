namespace API.Services.Authentication;

/// <summary>
/// Session data for mock authentication pagination
/// </summary>
public class MockSessionData
{
    public List<MockedUser> FilteredUsers { get; set; } = new();
    public int CurrentIndex { get; set; }
}