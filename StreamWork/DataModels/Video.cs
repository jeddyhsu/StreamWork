using System;
using System.Text.Json.Serialization;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Video : StorageBase
    {
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("view_count")]
        public float ViewCount { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("tag_ids")]
        public string[] TagIds { get; set; }

        [JsonPropertyName("chat_ids")]
        public string[] ChatIds { get; set; }

        [JsonPropertyName("comment_ids")]
        public string[] CommentIds { get; set; }
    }
}
