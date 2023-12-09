using core.Model;

namespace order.Model;

public class OrderedFoodItem
{
    public FoodItem Snapshot { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }
    
    public OrderedFoodItem(FoodItem snapshot, int count, string description)
    {
        Snapshot = snapshot;
        Count = count;
        Description = description;
    }
}