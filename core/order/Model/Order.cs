using core.Model;

namespace order.Model;

public class Order
{
    public OrderStatus Status { get; set; }
    public Guid Id { get; set; }
    public User Customer { get; set; }
    public User Restaurant { get; set; }
    public List<FoodItem> FoodItems { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;

    public void Confirm()
    {
        Status = OrderStatus.Preparing;
    }

    public void Cancel()
    {
        Status = OrderStatus.Canceled;
    }
}