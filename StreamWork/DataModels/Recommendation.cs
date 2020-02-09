using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels {
    public class Recommendation : IStorageBase<Recommendation> {
        [Key]
        public string Id { get; set; }
        public string Student { get; set; }
        public string Tutor { get; set; }
        public string Text { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<Recommendation> builder) {

        }
    }
}
