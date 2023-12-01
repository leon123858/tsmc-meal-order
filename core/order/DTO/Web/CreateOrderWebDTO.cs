namespace order.DTO.Web;

public class CreateOrderWebDTO
{
    public string MenuId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<int> FoodItemIds { get; set; }
}