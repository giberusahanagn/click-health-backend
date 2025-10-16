using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ClickHealthBackend.Enums;

namespace ClickHealthBackend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("password")]
        public string Password { get; set; } = null!;

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; } = null!;

        [BsonElement("specialty")]
        public string Specialty { get; set; } = null!;

        [BsonElement("territory")]
        public string Territory { get; set; } = null!;

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public UserStatus Status { get; set; } = UserStatus.Pending;

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; }

        [BsonElement("preferredLanguage")]
        public string PreferredLanguage { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        // --- OTP fields ---
        [BsonElement("totp")]
        public string? Totp { get; set; }   // OTP code

        [BsonElement("totpGeneratedAt")]
        public DateTime? TotpGeneratedAt { get; set; } // OTP timestamp

        [BsonElement("mustResetPassword")]
        public bool MustResetPassword { get; set; } = false;

        [BsonElement("lastPasswordChangeAt")]
        public DateTime? LastPasswordChangeAt { get; set; }
    }
}
