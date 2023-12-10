using core.Model;
using order.Model;

namespace order.DTO.Web;

public class OrderedFoodItemWebDTO
{
    public FoodItem Snapshot { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }

    public static explicit operator OrderedFoodItemWebDTO(OrderedFoodItem orderedFoodItem)
    {
        return new OrderedFoodItemWebDTO
        {
            Snapshot = orderedFoodItem.Snapshot,
            Count = orderedFoodItem.Count,
            Description = orderedFoodItem.Description
        };
    }
}