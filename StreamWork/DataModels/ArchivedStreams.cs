using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class ArchivedStreams : IStorageBase<ArchivedStreams>
    {
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string StreamID { get; set; }
        public string StreamThumbnail { get; set; }
        public string Subject { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<ArchivedStreams> builder)
        {

        }
    }
}
