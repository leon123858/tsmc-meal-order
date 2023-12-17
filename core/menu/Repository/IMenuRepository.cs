using menu.Models;
namespace menu.Repository
{
    public interface IMenuRepository
    {
        public Task<IEnumerable<Menu>> FindAllAsync(bool isTempMenu);

        public Task<IEnumerable<Menu>> FindAllByLocationAsync(string location, bool isTempMenu);

        public Task<Menu?> FindAsnyc(string id, bool isTempMenu);

        public Task InsertAsync(Menu menu, bool isTempMenu);

        public Task ReplaceAsync(Menu menu, bool isTempMenu);
    }
}
