using core.Model;

namespace menu.Models.DTO
{
    public class RecFoodItemDTO
    {
        public string MenuId { get; set; } = "";
        public string RestaurantName { get; set; } = "";
        public int Index { get; set; }
        public FoodItem FoodItem { get; set; } = new FoodItem();
    }
}
