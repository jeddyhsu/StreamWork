using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Notification : IStorageBase<Notification>
    {
        [Key]
        public string Id { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            
        }
    }
}
