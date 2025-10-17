using ClickHealthBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ClickHealthBackend.Models
{
    public class Content
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("content_id")]
        public string ContentId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("therapy")]
        public string Therapy { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("contentType")]
        public ContentType ContentType { get; set; }

        [BsonElement("fileUrl")]
        public string FileUrl { get; set; }

        [BsonElement("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonRepresentation(BsonType.String)]
        [BsonElement("status")]
        public ContentStatus Status { get; set; }

        [BsonElement("reviewDate")]
        public DateTime? ReviewDate { get; set; }

        [BsonElement("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("uploadedBy_user_id")]
        public string UploadedByUserId { get; set; }

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("metadata")]
        public BsonDocument Metadata { get; set; }
    }
}
