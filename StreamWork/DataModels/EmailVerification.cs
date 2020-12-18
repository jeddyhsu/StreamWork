using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class EmailVerification : IStorageBase<EmailVerification>
    {
        [Key]
        public string _Id { get; set; }
        public string EmailAddress { get; set; }
        public string VerificationCode { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<EmailVerification> builder)
        {

        }
    }
}
