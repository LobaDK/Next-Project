namespace API.DTO.Responses.Auth;

public record class AuthenticationError
{
    public required string ErrorCode { get; set; }
    public string? Message { get; set; }
}
