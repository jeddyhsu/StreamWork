using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models {
    public class Payment : IStorageBase<Payment> {
        [Key]
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public string PayerEmail { get; set; }
        public string Student { get; set; }
        public string Tutor { get; set; }
        public string PaymentType { get; set; }
        public decimal Val { get; set; }
        public DateTime TimeSent { get; set; }
        public bool Verified { get; set; }
        public string Error { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<Payment> builder) { }
    }
}
