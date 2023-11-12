namespace menu.Models.DTO
{
    public class MenuUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<FoodItem> FoodItems { get; set; } = new();
    }
}
