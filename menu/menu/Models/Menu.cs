namespace menu.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
