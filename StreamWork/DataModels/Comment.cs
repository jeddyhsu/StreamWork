using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Comment : StorageBase
    {
        [BsonElement("commenter_id")]
        public string CommenterId { get; set; }

        [BsonElement("recipient_id")]
        public string RecipientId { get; set; }

        [BsonElement("parent_id")]
        public string ParentId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }

        [BsonElement("edited")]
        public bool Edited { get; set; }

        public Comment(User commenter, User recipient, Comment parent, string message) : this(commenter.Id, recipient?.Id, parent?.Id, message) { }

        public Comment(string commenterId, string recipientId, string parentId, string message)
        {
            Id = Guid.NewGuid().ToString();
            CommenterId = commenterId;
            RecipientId = recipientId;
            ParentId = parentId;
            Message = message;
            Time = DateTime.UtcNow;
            Edited = false;
        }
    }
}
