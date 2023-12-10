using menu.Models;

namespace menu.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllMenuAsync();

        Task<Menu?> GetMenuAsync(string id);

        Task CreateMenuAsync(Menu menu);

        Task UpdateMenuAsync(Menu newMenu);

        Task<IEnumerable<Menu>> GetMenusByLocationAsync(string location);
    }
}
