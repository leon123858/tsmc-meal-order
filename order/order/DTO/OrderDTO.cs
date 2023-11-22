using core.Model;
using order.Attributes;
using order.Model;

namespace order.DTO;

public class OrderDTO
{
    [UpdateKey]
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
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