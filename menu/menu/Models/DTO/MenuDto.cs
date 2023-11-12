using menu.Models;

namespace menu.Models.DTO
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<FoodItem> FoodItems { get; set; } = new();
    }
}
