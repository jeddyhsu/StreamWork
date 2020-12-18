using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Channel : IStorageBase<Channel>
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Key { get; set; }
        public float ViewCount { get; set; }
        public bool Partnered { get; set; }
        public float Balance { get; set; }
        public string PayPalAddress { get; set; }
        public string[] ScheduledStreamIds { get; set; }
        public string StreamId { get; set; }
        public string[] VideoIds { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Channel> builder)
        {

        }
    }
}
