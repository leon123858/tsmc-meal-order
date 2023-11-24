namespace menu.Models.DTO
{
    public class MenuCreateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public List<FoodItemCreateDto> FoodItems { get; set; } = new ();
    }
}
