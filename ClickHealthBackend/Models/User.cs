using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using ClickHealthBackend.Enums;

namespace ClickHealthBackend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("role")]
        public UserRole Role { get; set; }

        [BsonElement("specialty")]
        public string Specialty { get; set; }

        [BsonElement("territory")]
        public string Territory { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("lastLoginAt")]
        public DateTime? LastLoginAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("preferredLanguage")]
        public string PreferredLanguage { get; set; }
    }
}
