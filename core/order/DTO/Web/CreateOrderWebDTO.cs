namespace order.DTO.Web;

public class CreateOrderWebDTO
{
    public DateTime OrderDate { get; set; }
    public string MealType { get; set; }
    public string MenuId { get; set; }
    public int FoodItemId { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }
}