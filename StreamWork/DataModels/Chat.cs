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
    }
}
