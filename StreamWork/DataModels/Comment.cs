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

        [BsonElement("edited")]
        public bool Edited { get; set; }
    }
}
