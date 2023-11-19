namespace order.Model;

public class Order
{
    public OrderStatus Status { get; set; }
    public Guid Id { get; set; }
    public List<FoodItem> FoodItems { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime CreateTime { get; set; }

    public void Confirm()
    {
        Status = OrderStatus.Preparing;
    }
}