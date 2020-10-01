using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Chat : IStorageBase<Chat>
    {
        //All the join with Profiles we can take out of the Notification DB (not doing it now since we just launched)

        [Key]
        public string Id { get; set; }
        public string ChatId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; } // join with Profile
        public string Message { get; set; }
        public string ProfilePicture { get; set; } //join with Profile
        public DateTime Date { get; set; }
        public string ChatColor { get; set; } //join with Profile
        public int TimeOffset { get; set; }
        public string ArchivedVideoId { get; set; }

        [NotMapped]
        public string DateString { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Chat> builder)
        {
            
        }
    }
}
