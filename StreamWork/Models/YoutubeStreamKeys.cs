using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models
{
    public class YoutubeStreamKeys : IStorageBase<YoutubeStreamKeys>
    {
        [Key]
        public string Id { get; set; }
        public string StreamKey { get; set; }
        public string Username { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<YoutubeStreamKeys> builder)
        {

        }
    }
}
