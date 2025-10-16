using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ClickHealthBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickHealthBackend.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // --- Core Collections ---
        // Assuming your models map directly to collection names
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

        // Placeholders for your other 9 collections (Example names provided)
        public IMongoCollection<object> Campaigns => _database.GetCollection<object>("Campaigns");
        public IMongoCollection<object> Contents => _database.GetCollection<object>("Contents");
        public IMongoCollection<object> Patients => _database.GetCollection<object>("Patients");
        public IMongoCollection<object> Hcps => _database.GetCollection<object>("Hcps");
        public IMongoCollection<object> Territories => _database.GetCollection<object>("Territories");
        public IMongoCollection<object> Roles => _database.GetCollection<object>("Roles");
        public IMongoCollection<object> Logs => _database.GetCollection<object>("Logs");
        public IMongoCollection<object> Reports => _database.GetCollection<object>("Reports");
        public IMongoCollection<object> Settings => _database.GetCollection<object>("Settings");
        // NOTE: Replace 'object' with your actual model class names (e.g., Campaign, Content, etc.)

        public MongoDbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            // Best practice: The MongoClient is already checked and configured in Program.cs
            if (string.IsNullOrEmpty(settings.Value.DatabaseName))
                throw new ArgumentNullException(nameof(settings.Value.DatabaseName), "MongoDB database name is missing.");

            _database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        }
    }
}
