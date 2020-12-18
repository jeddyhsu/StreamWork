using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class DebugLog : IStorageBase<DebugLog>
    {
        [Key]
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<DebugLog> builder)
        {

        }
    }
}
