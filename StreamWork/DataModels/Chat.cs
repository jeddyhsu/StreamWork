using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Chat : IStorageBase<Chat>
    {
        [Key]
        public string Id { get; set; }
        public string ChatId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime Date { get; set; }
        public string ChatColor { get; set; }
        public int TimeOffset { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Chat> builder)
        {
            
        }
    }
}
