namespace order.Model;

public class Order
{
    public OrderStatus Status { get; set; }
    public Guid Id { get; set; }
    public User Customer { get; set; }
    public User Restaurant { get; set; }
    public List<OrderedFoodItem> FoodItems { get; set; } = new();
    public DateTime OrderDate { get; set; }
    public MealType MealType { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;

    public void Confirm()
    {
        if (Status != OrderStatus.Init)
            throw new Exception("Order is not in correct status");

        Status = OrderStatus.Preparing;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Init)
            throw new Exception("Order is not in correct status");

        Status = OrderStatus.Canceled;
    }
}