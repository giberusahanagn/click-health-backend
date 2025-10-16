<<<<<<< Updated upstream
﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ClickHealthBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
=======
using ClickHealth.Server.Models;
using ClickHealthBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
>>>>>>> Stashed changes

namespace ClickHealthBackend.Data
{
    public class MongoDbContext
    {
<<<<<<< Updated upstream
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

=======
>>>>>>> Stashed changes
        public MongoDbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            if (mongoClient == null) throw new ArgumentNullException(nameof(mongoClient));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrEmpty(settings.Value.DatabaseName))
                throw new ArgumentNullException(nameof(settings.Value.DatabaseName), "MongoDB database name is missing.");

            Database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        }
<<<<<<< Updated upstream
=======

        public IMongoDatabase Database { get; }

        public IMongoCollection<User> Users => Database.GetCollection<User>("Users");
        public IMongoCollection<Campaign> Campaigns => Database.GetCollection<Campaign>("Campaigns");
        public IMongoCollection<Content> Contents => Database.GetCollection<Content>("Contents");
        public IMongoCollection<AuditLog> AuditLog => Database.GetCollection<AuditLog>("AuditLog");
        public IMongoCollection<CampaignAsset> CampaignAsset => Database.GetCollection<CampaignAsset>("CampaignAsset");
        public IMongoCollection<CampaignMetrics> CampaignMetric => Database.GetCollection<CampaignMetrics>("CampaignMetrics");
        public IMongoCollection<ConsentRecord> ConsentRecord => Database.GetCollection<ConsentRecord>("ConsentRecord");
        public IMongoCollection<ContentApproval> ContentApproval => Database.GetCollection<ContentApproval>("ContentApproval");
        public IMongoCollection<ContentEngagement> ContentEngagement => Database.GetCollection<ContentEngagement>("ContentEngagement");
        public IMongoCollection<HCPActivity> HCPActivity => Database.GetCollection<HCPActivity>("HCPActivity");
        public IMongoCollection<MRActivity> MRActivity => Database.GetCollection<MRActivity>("MRActivity");
        public IMongoCollection<PatientEngagement> PatientEngagement => Database.GetCollection<PatientEngagement>("PatientEngagement");
        public IMongoCollection<PatientInvite> PatientInvite => Database.GetCollection<PatientInvite>("PatientInvite");
>>>>>>> Stashed changes
    }
}
