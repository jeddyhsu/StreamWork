using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels {
    public class Comment : IStorageBase<Comment> {

        [Key]
        public string Id { get; set; }
        public string SenderUsername { get; set; }
        public string SenderName { get; set; }
        public string SenderProfilePicture { get; set; }
        public string ReceiverUsername { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string ParentId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<Comment> builder) {

        }
    }
}
