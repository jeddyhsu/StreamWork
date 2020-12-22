using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Tag : StorageBase
    {
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

        public Tag (string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Hidden = false;
            ChannelIds = new string[] { };
            ScheduledStreamIds = new string[] { };
            StreamIds = new string[] { };
            VideoIds = new string[] { };
        }
    }
}
