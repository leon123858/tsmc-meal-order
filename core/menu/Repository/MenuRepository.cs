using menu.Config;
using menu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace menu.Repository
{
    public class MenuRepository: IMenuRepository
    {
        private readonly IMongoCollection<Menu> _menus;
        private readonly IMongoCollection<Menu> _tmpMenus;
        private string _connectionString;

        public MenuRepository(IOptions<MenuDatabaseConfig> databaseSettings)
        {
            var secretPassword = Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? databaseSettings.Value.Password;

            _connectionString =
                $"mongodb+srv://{databaseSettings.Value.UserName}:{secretPassword}@{databaseSettings.Value.Cluser}";

            var mongoClient = new MongoClient(_connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _menus = mongoDatabase.GetCollection<Menu>(databaseSettings.Value.RealCollectionName);
            _tmpMenus = mongoDatabase.GetCollection<Menu>(databaseSettings.Value.TempCollectionName);
        }

        public async Task<IEnumerable<Menu>> FindAllAsync(bool isTempMenu)
        {
            if (isTempMenu)
                return await _tmpMenus.Find(_ => true).ToListAsync();

            return await _menus.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Menu>> FindAllByLocationAsync(string location, bool isTempMenu)
        {
            if (isTempMenu)
                return await _tmpMenus.Find(m => m.Location == location).ToListAsync();

            return await _menus.Find(m => m.Location == location).ToListAsync();
        }

        public async Task<Menu?> FindAsnyc(string id, bool isTempMenu)
        {
            if (isTempMenu)
                return await _tmpMenus.Find(m => m.Id == id).FirstOrDefaultAsync();

            return await _menus.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(Menu menu, bool isTempMenu)
        {
            if (isTempMenu)
                await _tmpMenus.InsertOneAsync(menu);
            else
                await _menus.InsertOneAsync(menu);
        }

        public async Task ReplaceAsync(Menu menu, bool isTempMenu)
        {
            if (isTempMenu)
                await _tmpMenus.ReplaceOneAsync(m => m.Id == menu.Id, menu);
            else 
                await _menus.ReplaceOneAsync(m => m.Id == menu.Id, menu);
        }
    }
}
