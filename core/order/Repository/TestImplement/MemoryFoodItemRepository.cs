using core.Model;

namespace order.Repository.TestImplement;

public class MemoryFoodItemRepository : IFoodItemRepository
{
    public Task<FoodItem> GetFoodItem(string menuId, int itemIdx)
    {
        return Task.FromResult(new FoodItem
        {
            Name = "Test Food Item",
            Price = 100,
            Count = 10,
            Tags = new List<string> { "Test", "Food", "Item" }
        });
    }

    public Task AdjustFoodItemStock(string menuId, int itemIndex, int quantity)
    {
        return Task.CompletedTask;
    }
}