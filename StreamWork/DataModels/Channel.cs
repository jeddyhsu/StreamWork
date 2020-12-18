using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Channel : IStorageBase<Channel>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

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
        
        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Channel> builder)
        {

        }
    }
}
