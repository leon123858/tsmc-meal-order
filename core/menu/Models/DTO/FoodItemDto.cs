namespace menu.Models.DTO
{
    public class FoodItemDto
    {
        public string Description { get; set; } = "";
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public int Count { get; set; }
        public string ImageUrl { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();
    }
}
