using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Tag : IStorageBase<Tag>
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("hidden")]
        public bool Hidden { get; set; }

        [BsonElement("channel_ids")]
        public string[] ChannelIds { get; set; }

        [BsonElement("scheduled_stream_ids")]
        public string[] ScheduledStreamIds { get; set; }

        [BsonElement("stream_ids")]
        public string[] StreamIds { get; set; }

        [BsonElement("video_ids")]
        public string[] VideoIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Tag> builder)
        {

        }
    }
}
