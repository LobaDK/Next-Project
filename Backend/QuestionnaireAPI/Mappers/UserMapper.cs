using System.Linq.Expressions;
using QuestionnaireDatabaseV2;
using QuestionnaireAPI.DTO.Responses.User;

namespace QuestionnaireAPI.Mappers;

/// <summary>
/// Mapper service for User entity to DTO conversions
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Expression for projecting User entity to UserDTO
    /// This can be used directly in LINQ queries for efficient database projections
    /// </summary>
    public static Expression<Func<QuestionnaireDatabaseV2.Entities.User, UserDTO>> ToUserDtoExpression =>
        u => new UserDTO
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        };
}