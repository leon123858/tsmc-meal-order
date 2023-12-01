using System.ComponentModel.DataAnnotations.Schema;
using order.Attributes;
using order.Model;

namespace order.DTO.Sql;

[Table("myUser")]
public class UserSqlDTO
{
    public string Id { get; set; }
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