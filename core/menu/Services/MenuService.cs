using core.Model;
using menu.Config;
using menu.Exceptions;
using menu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace menu.Services
{
    public class MenuService
    {
        private readonly IMongoCollection<Menu> _menusCollection;
        private string _connectionString;

        public MenuService(IOptions<MenuDatabaseConfig> databaseSettings)
        {
            var secretPassword = Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? databaseSettings.Value.Password;
            Console.WriteLine(secretPassword);

            _connectionString =
                $"mongodb+srv://{databaseSettings.Value.UserName}:{secretPassword}@{databaseSettings.Value.Cluser}";

            Console.WriteLine(_connectionString);

            var mongoClient = new MongoClient(_connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _menusCollection = mongoDatabase.GetCollection<Menu>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Menu>> GetAllAsync() =>
            await _menusCollection.Find(_ => true).ToListAsync();


        public async Task<Menu?> GetMenuAsync(string id)
        {
            var menu = await _menusCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (menu != null)
            {
                return menu;
            }

            throw new MenuNotFoundException();
        }

        public async Task<List<Menu>> GetMenusByLocationAsync(string location) =>
            await _menusCollection.Find(m => m.Location == location).ToListAsync();

        public async Task CreateMenuAsync(Menu menu) =>
            await _menusCollection.InsertOneAsync(menu);

        public async Task UpdateMenuAsync(Menu newMenu) =>
            await _menusCollection.ReplaceOneAsync(m => m.Id == newMenu.Id, newMenu);

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
