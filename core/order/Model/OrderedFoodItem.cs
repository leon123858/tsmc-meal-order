using core.Model;

namespace order.Model;

public class OrderedFoodItem
{
    public OrderedFoodItem(FoodItem snapshot, int index, int count, string? description)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than 0");
        if (index < 0)
            throw new ArgumentException("Index must be greater than or equal to 0");
        
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        Index = index;
        Count = count;
        Description = description ?? string.Empty;
    }

    public FoodItem Snapshot { get; }
    public int Index { get; }
    public int Count { get; }
    public string Description { get; }
}