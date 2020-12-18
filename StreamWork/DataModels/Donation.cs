using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Donation : IStorageBase<Donation>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("donor_id")]
        public string DonorId { get; set; }

        [JsonPropertyName("donee_id")]
        public string DoneeId { get; set; }

        [JsonPropertyName("value")]
        public float Value { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("transaction")]
        public object Transaction { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<Donation> builder)
        {

        }
    }
}
