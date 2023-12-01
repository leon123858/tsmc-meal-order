namespace menu.Models.DTO
{
    public class MenuDTO
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public List<FoodItemDTO> FoodItems { get; set; } = new();
    }
}
