using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Tag : IStorageBase<Tag>
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Hidden { get; set; }
        public string[] ChannelIds { get; set; }
        public string[] ScheduledStreamIds { get; set; }
        public string[] StreamIds { get; set; }
        public string[] VideoIds { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Tag> builder)
        {

        }
    }
}
