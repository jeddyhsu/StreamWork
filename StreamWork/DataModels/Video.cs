using System;
using System.Linq;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Video : StorageBase
    {
        [BsonElement("channel_id")]
        public string ChannelId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("start_time")]
        public DateTime StartTime { get; set; }

        [BsonElement("thumbnail")]
        public string Thumbnail { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("view_count")]
        public float ViewCount { get; set; }

        [BsonElement("topic")]
        public string Topic { get; set; }

        [BsonElement("tag_ids")]
        public string[] TagIds { get; set; }

        [BsonElement("chat_ids")]
        public string[] ChatIds { get; set; }

        [BsonElement("comment_ids")]
        public string[] CommentIds { get; set; }

        public Video(Channel channel, string title, DateTime startTime, string thumbnail, string description, string topic, Tag[] tags, Chat[] chats)
            : this(channel.Id, title, startTime, thumbnail, description, topic, tags.ToList().Select(tag => tag.Id).ToArray(), chats.ToList().Select(tag => tag.Id).ToArray()) { }

        public Video(string channelId, string title, DateTime startTime, string thumbnail, string description, string topic, string[] tagIds, string[] chatIds)
        {
            Id = Guid.NewGuid().ToString();
            ChannelId = channelId;
            Title = title;
            StartTime = startTime;
            Thumbnail = thumbnail;
            Description = description;
            ViewCount = 0;
            Topic = topic;
            TagIds = tagIds;
            ChatIds = chatIds;
            CommentIds = new string[] { };
        }
    }
}
