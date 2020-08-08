using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class TopicTutor : IStorageBase<TopicTutor>
    {
        [Key]
        public string Id { get; set; }
        public string Tutor { get; set; }
        public string Topic { get; set; }
        public DateTime Since { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<TopicTutor> builder)
        {

        }
    }
}
