using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class DebugLog : IStorageBase<DebugLog>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<DebugLog> builder)
        {

        }
    }
}
