using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Channel : StorageBase
    {
        [BsonElement("key")]
        public string Key { get; set; }

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

        public Channel (string key, string payPalAddress)
        {
            Id = Guid.NewGuid().ToString();
            Key = key;
            ViewCount = 0;
            Partnered = false;
            Balance = 0;
            PayPalAddress = payPalAddress;
            ScheduledStreamIds = new string[] { };
            StreamId = null;
            VideoIds = new string[] { };
        }
    }
}
