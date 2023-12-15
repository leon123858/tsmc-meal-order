namespace menu.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("User does not exist.")
    {
    }
}