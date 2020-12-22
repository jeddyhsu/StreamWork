﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class ScheduledStream : IStorageBase<ScheduledStream>
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("channel_id")]
        public string ChannelId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("start_time")]
        public DateTime StartTime { get; set; }

        [BsonElement("end_time")]
        public DateTime EndTime { get; set; }

        [BsonElement("thumbnail")]
        public string Thumbnail { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("topic")]
        public string Topic { get; set; }

        [BsonElement("tag_ids")]
        public string[] TagIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<ScheduledStream> builder)
        {

        }
    }
}
