using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class SignUpProgress : IStorageBase<SignUpProgress>
    {
        [Key]
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string LatestTab { get; set; }
        public DateTime Date { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<SignUpProgress> builder)
        {

        }
    }
}
