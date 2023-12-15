namespace menu.Exceptions;

public class FoodItemNotFoundException : Exception
{
    public FoodItemNotFoundException() : base("Food item does not exist.")
    {
    }
}