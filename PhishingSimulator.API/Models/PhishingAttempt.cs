using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PhishingSimulator.API.Models
{
    public class PhishingAttempt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("recipientEmail")]
        public string RecipientEmail { get; set; } = string.Empty;
        
        [BsonElement("emailContent")]
        public string EmailContent { get; set; } = string.Empty;
        
        [BsonElement("trackingLink")]
        public string TrackingLink { get; set; } = string.Empty;
        
        [BsonElement("isClicked")]
        public bool IsClicked { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("clickedAt")]
        public DateTime? ClickedAt { get; set; }
    }
} 