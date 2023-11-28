using order.Model;

namespace order.DTO.Sql;

public class UserSqlDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; }

    public static implicit operator User(UserSqlDTO sqlDto)
    {
        return new User
        {
            Id = sqlDto.Id,
            Email = sqlDto.Email
        };
    }

    public static explicit operator UserSqlDTO(User user)
    {
        return new UserSqlDTO
        {
            Id = user.Id,
            Email = user.Email
        };
    }
}