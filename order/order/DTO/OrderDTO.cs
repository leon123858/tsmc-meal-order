using core.Model;

namespace order.DTO;

public class OrderDTO
{
    public Guid RestaurantId { get; set; }
    public List<FoodItem> FoodItems { get; set; }
}