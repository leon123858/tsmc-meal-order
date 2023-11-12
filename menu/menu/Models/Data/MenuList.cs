namespace menu.Models.Data
{
    public static class MenuList
    {
        public static List<Menu> menuList = new(){
            new Menu{Id = 1, Name = "Store1", FoodItems = new List<FoodItem>()},
            new Menu{Id = 2, Name = "Store2", FoodItems = new List<FoodItem>()}
        };
        
        public static int generateNewId()
        {
            return menuList.OrderByDescending(m => m.Id).FirstOrDefault()!.Id + 1;
        }

    }
}
