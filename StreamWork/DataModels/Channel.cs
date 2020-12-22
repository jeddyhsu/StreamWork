using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Channel : StorageBase
    {
        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("view_count")]
        public float ViewCount { get; set; }

        [BsonElement("partnered")]
        public bool Partnered { get; set; }

        [BsonElement("balance")]
        public float Balance { get; set; }

        [BsonElement("paypal_address")]
        public string PayPalAddress { get; set; }

        [BsonElement("scheduled_stream_ids")]
        public string[] ScheduledStreamIds { get; set; }

        [BsonElement("stream_id")]
        public string StreamId { get; set; }

        [BsonElement("video_ids")]
        public string[] VideoIds { get; set; } 
    }
}
