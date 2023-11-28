namespace order.DTO.Web;

public class CreateOrderWebDTO
{
    public Guid RestaurantId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<int> FoodItemIds { get; set; }
}