namespace menu.Models.DTO
{
    public class MenuCreateDto
    {
        public string Name { get; set; } = "";
        public List<FoodItem> FoodItems { get; set; } = new ();
    }
}
