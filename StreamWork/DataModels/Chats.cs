using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Chats : IStorageBase<Chats>
    {
        [Key]
        public string Id { get; set; }
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime Date { get; set; }
        public string ChatColor { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Chats> builder)
        {
            
        }
    }
}
