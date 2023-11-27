using menu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace menu.Services
{
    public class MenuService
    {
        private readonly IMongoCollection<Menu> _menusCollection;

        public MenuService(IOptions<MenuDatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _menusCollection = mongoDatabase.GetCollection<Menu>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Menu>> GetAsync() =>
            await _menusCollection.Find(_ => true).ToListAsync();

        public async Task<Menu?> GetAsync(string id) =>
            await _menusCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Menu menu) =>
            await _menusCollection.InsertOneAsync(menu);

        public async Task UpdateAsync(Menu newMenu) =>
            await _menusCollection.ReplaceOneAsync(m => m.Id == newMenu.Id, newMenu);
    }
}
