using System.Text.Json.Serialization;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Chat : StorageBase
    {
        [JsonPropertyName("commenter_id")]
        public string CommenterId { get; set; }

        [JsonPropertyName("recipient_id")]
        public string RecipientId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
