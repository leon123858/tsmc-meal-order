namespace menu.Models.DTO
{
    public class MenuDto
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public List<FoodItemDto> FoodItems { get; set; } = new();
    }
}
