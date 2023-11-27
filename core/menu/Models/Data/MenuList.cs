using core.Model;

namespace menu.Models.Data
{
    public static class MenuList
    {
        public static List<Menu> menuList = new(){
            new Menu{Id = Guid.NewGuid(), Name = "Store1", FoodItems = new List<FoodItem>()
            {
                new FoodItem() {Name = "Food1", Price = 10},
                new FoodItem() {Name = "Food2", Price = 20},
            }},
            new Menu{Id = Guid.NewGuid(), Name = "Store2", FoodItems = new List<FoodItem>()}
        };
        
        public static Guid generateNewId()
        {
            return Guid.NewGuid();
        }

    }
}
