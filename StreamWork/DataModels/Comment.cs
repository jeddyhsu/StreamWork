using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels {
    public class Comment : IStorageBase<Comment> {

        [NotMapped]
        public List<Comment> Replies = new List<Comment>();

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
        public string StreamId { get; set; }
        public string Edited { get; set; }
        public string ProfileColor { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<Comment> builder) {

        }
    }
}
