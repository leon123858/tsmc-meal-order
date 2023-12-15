namespace order.Model;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Place { get; set; }
    public UserType Type { get; set; }
}