namespace order.Model;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Place { get; set; }
    public string UserType { get; set; }
}