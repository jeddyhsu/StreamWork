using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class View : IStorageBase<View>
    {
        [Key]
        //Both
        public string Id { get; set; }
        public string Viewer { get; set; }
        public string Channel { get; set; }
        public string StreamId { get; set; }
        public DateTime Date { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<View> builder)
        {

        }
    }
}
