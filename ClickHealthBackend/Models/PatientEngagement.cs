
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class PatientEngagement 
    {
        [BsonElement("inviteCode")]
        [BsonRepresentation(BsonType.ObjectId)]

        public string InviteCode { get; set; } = string.Empty;

        [BsonElement("pseudonymousId")]
        public string PseudonymousId { get; set; } = string.Empty;

        [BsonElement("contentId")]
        public string ContentId { get; set; } = string.Empty;

        [BsonElement("campaignId")]
        public string? CampaignId { get; set; }

        [BsonElement("viewedAt")]
        public DateTime? ViewedAt { get; set; }

        [BsonElement("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [BsonElement("durationSeconds")]
        public int DurationSeconds { get; set; }

        [BsonElement("consentGiven")]
        public bool ConsentGiven { get; set; }

        [BsonElement("quizResponse")]
        public Dictionary<string, object>? QuizResponse { get; set; }

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("language")]
        public string? Language { get; set; }
    }
}
