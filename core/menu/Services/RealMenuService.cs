using core.Model;
using menu.Config;
using menu.Exceptions;
using menu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace menu.Services
{
    public class RealMenuService: IMenuService
    {
        private readonly IMongoCollection<Menu> _menus;
        private string _connectionString;

        public RealMenuService(IOptions<MenuDatabaseConfig> databaseSettings)
        {
            var secretPassword = Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? databaseSettings.Value.Password;

            _connectionString =
                $"mongodb+srv://{databaseSettings.Value.UserName}:{secretPassword}@{databaseSettings.Value.Cluser}";

            var mongoClient = new MongoClient(_connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _menus = mongoDatabase.GetCollection<Menu>(databaseSettings.Value.RealCollectionName);
        }

        public async Task<IEnumerable<Menu>> GetAllMenuAsync()
        {
            return await _menus.Find(_ => true).ToListAsync();
        }

        public async Task<Menu?> GetMenuAsync(string id)
        {
            var menu = await _menus.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (menu != null)
            {
                return menu;
            }

            throw new MenuNotFoundException();
        }

        public async Task<IEnumerable<Menu>> GetMenusByLocationAsync(string location) =>
            await _menus.Find(m => m.Location == location).ToListAsync();

        public async Task CreateMenuAsync(Menu menu) =>
            await _menus.InsertOneAsync(menu);

        public async Task UpdateMenuAsync(Menu newMenu) =>
            await _menus.ReplaceOneAsync(m => m.Id == newMenu.Id, newMenu);

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
