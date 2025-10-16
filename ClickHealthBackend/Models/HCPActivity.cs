using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using ClickHealthBackend.Enums; 

namespace ClickHealthBackend.Models
{
    public class HCPActivity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string hcpActivityId { get; set; }

        [BsonElement("hcpUserId")]
        public string HcpUserId { get; set; }

        [BsonElement("hcpActivityType")]
        [BsonRepresentation(BsonType.String)]
        public HCPActivityType hcpActivityType { get; set; }

        [BsonElement("contentId")]
        public string ContentId { get; set; }

        [BsonElement("campaignId")]
        public string CampaignId { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("durationSeconds")]
        public int DurationSeconds { get; set; }

        [BsonElement("metadata")]
        public object Metadata { get; set; }

        [BsonElement("territory")]
        public string Territory { get; set; }

        [BsonElement("city")]
        public string City { get; set; }
    }
}