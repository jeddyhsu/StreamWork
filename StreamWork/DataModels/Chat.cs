using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Chat : IStorageBase<Chat>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("commenter_id")]
        public string CommenterId { get; set; }

        [JsonPropertyName("recipient_id")]
        public string RecipientId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Chat> builder)
        {
            
        }
    }
}
