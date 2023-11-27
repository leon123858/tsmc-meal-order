using core.Model;

namespace order.Repository;

public interface IFoodItemRepository
{
    Task<FoodItem> GetFoodItem(Guid menuId, int itemIdx);
}