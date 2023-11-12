namespace menu.Models.DTO
{
    public class MenuCreateDto
    {
        public string Name { get; set; } = "";
        public List<FoodItemCreateDto> FoodItems { get; set; } = new ();
    }
}
