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
        public string CommenterId { get; set; }
        public string RecipientId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Chat> builder)
        {
            
        }
    }
}
