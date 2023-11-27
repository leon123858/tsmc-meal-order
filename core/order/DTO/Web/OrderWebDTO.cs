namespace order.DTO.Web;

public class OrderWebDTO
{
    public Guid RestaurantId { get; set; }
    public List<int> FoodItemIds { get; set; }
}