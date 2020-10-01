using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Notification : IStorageBase<Notification>
    {
        //All the join with Profiles we can take out of the Notification DB (not doing it now since we just launched)

        [Key]
        public string Id { get; set; }
        public string SenderUsername { get; set; }
        public string SenderName { get; set; } //join with Profile
        public string SenderProfilePicture { get; set; } //join with Profile
        public string ReceiverUsername { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public string Seen { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string NotificationInfo { get; set; }
        public string ObjectId { get; set; }
        public string ProfileColor { get; set; } //join with Profile

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            
        }
    }
}
