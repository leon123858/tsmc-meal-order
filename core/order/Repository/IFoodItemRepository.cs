using core.Model;

namespace order.Repository;

public interface IFoodItemRepository
{
    Task<FoodItem> GetFoodItem(string menuId, int itemIdx);
}