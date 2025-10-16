
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class PatientInvite
    {
        [BsonElement("inviteCode")]
        [BsonRepresentation(BsonType.ObjectId)]

        public string InviteCode { get; set; } = string.Empty;

        [BsonElement("hcpUserId")]
        public string HcpUserId { get; set; } = string.Empty;

        [BsonElement("contentId")]
        public string ContentId { get; set; } = string.Empty;

        [BsonElement("campaignId")]
        public string? CampaignId { get; set; }

        [BsonElement("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("maxUses")]
        public int MaxUses { get; set; } = 100;

        [BsonElement("usedCount")]
        public int UsedCount { get; set; }

        [BsonElement("shareChannel")]
        public string? ShareChannel { get; set; }
    }
}
