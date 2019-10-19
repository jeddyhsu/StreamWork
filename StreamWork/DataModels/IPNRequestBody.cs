using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models {
    public class IPNRequestBody : IStorageBase<IPNRequestBody> {
        [Key]
        public string Id { get; set; }
        public string RequestBody { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<IPNRequestBody> builder) { }
    }
}
