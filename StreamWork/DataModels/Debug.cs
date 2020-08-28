using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Debug : IStorageBase<Debug>
    {
        [Key]
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Debug> builder)
        {

        }
    }
}
