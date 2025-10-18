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
        [BsonRepresentation(BsonType.String)]
        [BsonElement("content_id")]
        public string ContentId { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("medical_name")]
        public string Therapy { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("contentType")]
        public ContentType ContentType { get; set; }

        [BsonElement("upload_pdf")]
        public string FileUrl { get; set; }

        [BsonElement("video_url")]
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

        [BsonRepresentation(BsonType.String)]
        [BsonElement("uploadedBy_user_id")]
        public string UploadedByUserId { get; set; }

        public string UploadedByUserName { get; set; }//added username for reference

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("metadata")]
        public BsonDocument Metadata { get; set; }


        // New fields for approval workflow
        public string ApprovedByUserId { get; set; }
        public string ApprovedByUserName { get; set; }
        public string ApproverNotes { get; set; }
    }
}
