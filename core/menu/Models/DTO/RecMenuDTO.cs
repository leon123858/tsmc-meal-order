namespace menu.Models.DTO
{
    public class RecMenuDTO
    {
        public string MenuId { get; set; } = "";
        public List<FoodItemDTO> FoodItems { get; set; } = new();
    }
}
