using core.Model;
using menu.Exceptions;
using menu.Models;
using menu.Repository;

namespace menu.Services
{
    public class MenuService: IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository= menuRepository;
        }

        public async Task<IEnumerable<Menu>> GetAllMenuAsync(bool isTempMenu)
        {
            return await _menuRepository.FindAllAsync(isTempMenu);
        }

        public async Task<Menu?> GetMenuAsync(string id, bool isTempMenu)
        {
            var menu = await _menuRepository.FindAsnyc(id, isTempMenu);
            if (menu != null)
            {
                return menu;
            }

            throw new MenuNotFoundException();
        }

        public async Task<IEnumerable<Menu>> GetMenusByLocationAsync(string location, bool isTempMenu)
        {
            return await _menuRepository.FindAllAsyncByLocationAsync(location, isTempMenu);
        }

        public async Task CreateMenuAsync(Menu menu, bool isTempMenu)
        {
            await _menuRepository.InsertAsync(menu, isTempMenu);
        }

        public async Task UpdateMenuAsync(Menu newMenu, bool isTempMenu)
        {
            await _menuRepository.ReplaceAsync(newMenu, isTempMenu);
        }

        public FoodItem? GetFoodItem(Menu? menu, int itemIdx)
        {
            if (itemIdx < menu!.FoodItems.Count)
            {
                return menu!.FoodItems[itemIdx];
            }

            throw new FoodItemNotFoundException();
        }
    }
}
