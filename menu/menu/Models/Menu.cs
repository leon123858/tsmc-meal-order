using core.Model;

namespace menu.Models
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public List<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
