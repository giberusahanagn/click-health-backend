using ClickHealthBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClickHealthBackend.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Expose the database for creating collections
        public IMongoDatabase Database => _database;

        // Collections
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Campaign> Campaigns => _database.GetCollection<Campaign>("Campaigns");
        public IMongoCollection<Content> Contents => _database.GetCollection<Content>("Contents");
    }
}
