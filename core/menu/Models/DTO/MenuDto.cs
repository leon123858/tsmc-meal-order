namespace menu.Models.DTO
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public List<FoodItemDto> FoodItems { get; set; } = new();
    }
}
