using menu.Config;
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

        public async Task<Menu?> GetByIdAsync(string id) =>
            await _menusCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        public async Task<List<Menu>> GetByLocationAsync(string location) =>
            await _menusCollection.Find(m => m.Location == location).ToListAsync();

        public async Task CreateAsync(Menu menu) =>
            await _menusCollection.InsertOneAsync(menu);

        public async Task UpdateAsync(Menu newMenu) =>
            await _menusCollection.ReplaceOneAsync(m => m.Id == newMenu.Id, newMenu);
    }
}
