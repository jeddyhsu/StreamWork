using System.Text.Json.Serialization;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Channel : StorageBase
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("view_count")]
        public float ViewCount { get; set; }

        [JsonPropertyName("partnered")]
        public bool Partnered { get; set; }

        [JsonPropertyName("balance")]
        public float Balance { get; set; }

        [JsonPropertyName("paypal_address")]
        public string PayPalAddress { get; set; }

        [JsonPropertyName("scheduled_stream_ids")]
        public string[] ScheduledStreamIds { get; set; }

        [JsonPropertyName("stream_id")]
        public string StreamId { get; set; }

        [JsonPropertyName("video_ids")]
        public string[] VideoIds { get; set; } 
    }
}
