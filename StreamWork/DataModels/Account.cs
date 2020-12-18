using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Account : IStorageBase<Account>
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ChangePasswordKey { get; set; }
        public bool SubscribedToEmail { get; set; }
        public string TimeZone { get; set; }
        public DateTime SignUpDate { get; set; }
        public string[] Notifications { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Account> builder)
        {

        }
    }
}
