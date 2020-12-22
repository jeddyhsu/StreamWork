using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class DebugLog : IStorageBase<DebugLog>
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<DebugLog> builder)
        {

        }
    }
}
