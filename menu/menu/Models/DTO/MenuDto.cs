namespace menu.Models.DTO
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<FoodItemDto> FoodItems { get; set; } = new();
    }
}
