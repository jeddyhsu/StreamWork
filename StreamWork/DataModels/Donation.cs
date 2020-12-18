using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Donation : IStorageBase<Donation>
    {
        [Key]
        public string Id { get; set; }
        public string DonorId { get; set; }
        public string DoneeId { get; set; }
        public float Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TransactionId { get; set; }
        public object Transaction { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<Donation> builder)
        {

        }
    }
}
