namespace menu.Models.DTO
{
    public class MenuCreateDTO
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public List<FoodItemCreateDTO> FoodItems { get; set; } = new ();
    }
}
