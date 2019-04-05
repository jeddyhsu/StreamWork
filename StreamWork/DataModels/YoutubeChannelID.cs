using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models
{
    public class YoutubeChannelID : IStorageBase<YoutubeChannelID>
    {
        [Key]
        public string Id { get; set; }
        public string ChannelKey { get; set; }
        public string ChannelKeyAPI { get; set; }
        public string Username { get; set; }
        public string SubjectStreaming { get; set; }
        public string StreamThumbnail { get; set; }
        public string StreamID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<YoutubeChannelID> builder)
        {

        }
    }
}
