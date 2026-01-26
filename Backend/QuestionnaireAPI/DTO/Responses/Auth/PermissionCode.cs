namespace QuestionnaireAPI.DTO.Responses.Auth;

public record class PermissionCode
{
    public required string Name { get; set; }
    public required int Value { get; set; }
}
