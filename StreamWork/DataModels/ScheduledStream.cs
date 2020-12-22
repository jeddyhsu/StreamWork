using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class ScheduledStream : StorageBase
    {
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

        public ScheduledStream(Channel channel, string title, DateTime startTime, DateTime endTime, string thumbnail, string description, string topic, Tag[] tags)
            : this(channel.Id, title, startTime, endTime, thumbnail, description, topic, tags.ToList().Select(tag => tag.Id).ToArray()) { }

        public ScheduledStream (string channelId, string title, DateTime startTime, DateTime endTime, string thumbnail, string description, string topic, string[] tagIds)
        {
            Id = Guid.NewGuid().ToString();
            ChannelId = channelId;
            Title = title;
            StartTime = startTime;
            EndTime = endTime;
            Thumbnail = thumbnail;
            Description = description;
            Topic = topic;
            TagIds = tagIds;
        }
    }
}
