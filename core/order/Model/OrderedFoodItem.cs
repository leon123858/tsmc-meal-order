using core.Model;

namespace order.Model;

public class OrderedFoodItem
{
    public FoodItem Snapshot { get; set; }
    public int Index { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }
    
    public OrderedFoodItem(FoodItem snapshot, int index, int count, string description)
    {
        Snapshot = snapshot;
        Index = index;
        Count = count;
        Description = description;
    }
}