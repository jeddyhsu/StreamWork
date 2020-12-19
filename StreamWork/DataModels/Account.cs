using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Account : StorageBase
    {
        [JsonPropertyName("change_password_key")]
        public string ChangePasswordKey { get; set; }

        [JsonPropertyName("subscribed_to_email")]
        public bool SubscribedToEmail { get; set; }

        [JsonPropertyName("time_zone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("sign_up_date")]
        public DateTime SignUpDate { get; set; }

        [JsonPropertyName("notification_ids")]
        public string[] NotificationIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Account> builder)
        {

        }
    }
}
