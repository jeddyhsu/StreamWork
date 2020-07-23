using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Notification : IStorageBase<Notification>
    {
        [Key]
        public string Id { get; set; }
        public string SenderUsername { get; set; }
        public string SenderName { get; set; }
        public string SenderProfilePicture { get; set; }
        public string ReceiverUsername { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public string Seen { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string NotificationInfo { get; set; }
        public string ObjectId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            
        }
    }
}
