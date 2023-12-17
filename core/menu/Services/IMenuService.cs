using core.Model;
using menu.Models;

namespace menu.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllMenuAsync(bool isTempMenu);

        Task<Menu?> GetMenuAsync(string id, bool isTempMenu);

        Task CreateMenuAsync(Menu menu, bool isTempMenu);

        Task UpdateMenuAsync(Menu newMenu, bool isTempMenu);

        Task<IEnumerable<Menu>> GetMenusByLocationAsync(string location, int topK, bool isTempMenu);

        FoodItem? GetFoodItem(Menu? menu, int itemIdx);
    }
}
