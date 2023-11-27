using order.Model;

namespace order.DTO.Web;

public class UserWebDTO
{
    public string uid { get; set; }
    public string email { get; set; }
    public string name { get; set; }
    public string place { get; set; }
    public string userType { get; set; }
    
    public static implicit operator User(UserWebDTO webDto)
    {
        return new User
        {
            Id = Guid.Parse(webDto.uid),
            Email = webDto.email,
            Name = webDto.name,
            Place = webDto.place,
            UserType = webDto.userType
        };
    }
}