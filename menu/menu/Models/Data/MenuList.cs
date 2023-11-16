namespace menu.Models.Data
{
    public static class MenuList
    {
        public static List<Menu> menuList = new(){
            new Menu{Id = Guid.NewGuid(), Name = "Store1", FoodItems = new List<FoodItem>()},
            new Menu{Id = Guid.NewGuid(), Name = "Store2", FoodItems = new List<FoodItem>()}
        };
        
        public static Guid generateNewId()
        {
            return Guid.NewGuid();
        }

    }
}
