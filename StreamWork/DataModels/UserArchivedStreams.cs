using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class UserArchivedStreams : IStorageBase<UserArchivedStreams>
    {
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string StreamID { get; set; }
        public string StreamThumbnail { get; set; }
        public string StreamSubject { get; set; }
        public string StreamTitle { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime StartTime { get; set; }
        public int Views { get; set; }
        public string StreamDescription { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<UserArchivedStreams> builder)
        {

        }
    }
}
