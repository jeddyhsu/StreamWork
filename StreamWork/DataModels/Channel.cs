using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Channel : IStorageBase<Channel>
    {

        [NotMapped]
        public string StreamSubjectIcon { get; set; }

        //Tutors Only
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string ChannelKey { get; set; }
        public string StreamSubject { get; set; }
        public string StreamTitle { get; set; }
        public string StreamThumbnail { get; set; }
        public string StreamDescription { get; set; }
        public string ProfilePicture { get; set; }
        public string Name { get; set; }
        public string StreamColor { get; set; }
        public string ArchivedVideoId { get; set; }

        public DateTime StartTime { get; set; }
        public int Views { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Channel> builder)
        {

        }
    }
}
