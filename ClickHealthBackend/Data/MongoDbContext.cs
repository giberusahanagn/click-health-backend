using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ClickHealthBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public MongoDbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            // Best practice: The MongoClient is already checked and configured in Program.cs
            if (string.IsNullOrEmpty(settings.Value.DatabaseName))
                throw new ArgumentNullException(nameof(settings.Value.DatabaseName), "MongoDB database name is missing.");

            _database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        }
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

        public IMongoDatabase Database { get; internal set; }
    }
}