using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Chat : StorageBase
    {
        [BsonElement("stream_id")]
        public string StreamId { get; set; }

        [BsonElement("commenter_id")]
        public string CommenterId { get; set; }

        [BsonElement("recipient_id")]
        public string RecipientId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }

        public Chat (Stream stream, User commenter, User recipient, string message) : this(stream.Id, commenter.Id, recipient?.Id, message) { }

        public Chat (string streamId, string commenterId, string recipientId, string message)
        {
            Id = Guid.NewGuid().ToString();
            StreamId = streamId;
            CommenterId = commenterId;
            RecipientId = recipientId;
            Message = message;
            Time = DateTime.UtcNow;
        }
    }
}
