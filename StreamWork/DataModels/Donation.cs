using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models {
    public class Donation : IStorageBase<Donation> {
        [Key]
        public string Id { get; set; }
        public string Student { get; set; }
        public string Tutor { get; set; }
        public string Val { get; set; }
        public string TimeSent { get; set; }
        public string Verified { get; set; }
        public string Error { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<Donation> builder) { }
    }
}
