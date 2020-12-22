using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Donation : IStorageBase<Donation>
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("donor_id")]
        public string DonorId { get; set; }

        [BsonElement("donee_id")]
        public string DoneeId { get; set; }

        [BsonElement("value")]
        public float Value { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("transaction_id")]
        public string TransactionId { get; set; }

        [BsonElement("transaction")]
        public object Transaction { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<Donation> builder)
        {

        }
    }
}
