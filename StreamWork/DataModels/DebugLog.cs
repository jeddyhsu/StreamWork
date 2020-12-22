using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class DebugLog : StorageBase
    {
        [BsonElement("time")]
        public DateTime Time { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<DebugLog> builder)
        {

        }
    }
}
