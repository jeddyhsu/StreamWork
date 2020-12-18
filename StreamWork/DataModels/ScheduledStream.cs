using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class ScheduledStream : IStorageBase<ScheduledStream>
    {
        [Key]
        public string Id { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public string[] TagIds { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<ScheduledStream> builder)
        {

        }
    }
}
