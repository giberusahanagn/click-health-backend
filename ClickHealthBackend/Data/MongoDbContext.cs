using ClickHealth.Server.Models;
using ClickHealthBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics;

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
        public IMongoCollection<AuditLog> AuditLog => _database.GetCollection<AuditLog>("AuditLog");
        public IMongoCollection<CampaignAsset> CampaignAsset => _database.GetCollection<CampaignAsset>("CampaignAsset");
        public IMongoCollection<CampaignMetrics> CampaignMetric => _database.GetCollection<CampaignMetrics>("CampaignMetrics");
        public IMongoCollection<ConsentRecord> ConsentRecord => _database.GetCollection<ConsentRecord>("ConsentRecord");
        public IMongoCollection<ContentApproval> ContentApproval => _database.GetCollection<ContentApproval>("ContentApproval");
        public IMongoCollection<ContentEngagement> ContentEngagement => _database.GetCollection<ContentEngagement>("ContentEngagement");
        public IMongoCollection<HCPActivity> HCPActivity => _database.GetCollection<HCPActivity>("HCPActivity");
        public IMongoCollection<MRActivity> MRActivity => _database.GetCollection<MRActivity>("MRActivity");
        public IMongoCollection<PatientEngagement> PatientEngagement => _database.GetCollection<PatientEngagement>("PatientEngagement");
        public IMongoCollection<PatientInvite> PatientInvite => _database.GetCollection<PatientInvite>("PatientInvite");

 
    }
}
