using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Tag : IStorageBase<Tag>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("channel_ids")]
        public string[] ChannelIds { get; set; }

        [JsonPropertyName("scheduled_stream_ids")]
        public string[] ScheduledStreamIds { get; set; }

        [JsonPropertyName("stream_ids")]
        public string[] StreamIds { get; set; }

        [JsonPropertyName("video_ids")]
        public string[] VideoIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Tag> builder)
        {

        }
    }
}
