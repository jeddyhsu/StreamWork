using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Schedule : IStorageBase<Schedule>
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string StreamTitle { get; set; }
        public string StreamSubject { get; set; }
        public string StreamDescription { get; set; }
        public string StreamThumbnail { get; set; }
        public string TimeStart { get; set; }
        public string TimeStop { get; set; }
        public string TimeZone { get; set; }
        public DateTime Date { get; set; }
        public string SubjectThumbnail { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Schedule> builder)
        {

        }
    }
}
