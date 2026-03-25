namespace API.Interfaces;

public interface IAuthService
{
    Task<IActionResult> Login(UserLogin userLogin);
    Task<IActionResult> Refresh(RefreshRequest request, ClaimsPrincipal user, string? authorizationHeader);
    Task<IActionResult> Logout(string? authorizationHeader);
    IActionResult WhoAmI(string? authorizationHeader);
}
