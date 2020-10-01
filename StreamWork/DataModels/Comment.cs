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

        //All the join with Profiles we can take out of the Comment DB (not doing it now since we just launched)

        [Key]
        public string Id { get; set; }
        public string SenderUsername { get; set; }
        public string SenderName { get; set; } //join with Profile
        public string SenderProfilePicture { get; set; } //join with Profile
        public string ReceiverUsername { get; set; } 
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string ParentId { get; set; }
        public string StreamId { get; set; }
        public string Edited { get; set; }
        public string ProfileColor { get; set; } //join with Profile

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure (EntityTypeBuilder<Comment> builder) {

        }
    }
}
