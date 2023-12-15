namespace menu.Exceptions;

public class MenuNotFoundException : Exception
{
    public MenuNotFoundException() : base("Menu does not exist.")
    {
    }
}