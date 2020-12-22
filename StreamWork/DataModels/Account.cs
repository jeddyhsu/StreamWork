using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Account : StorageBase
    {
        [BsonElement("change_password_key")]
        public string ChangePasswordKey { get; set; }

        [BsonElement("subscribed_to_email")]
        public bool SubscribedToEmail { get; set; }

        [BsonElement("time_zone")]
        public string TimeZone { get; set; }

        [BsonElement("sign_up_date")]
        public DateTime SignUpDate { get; set; }

        [BsonElement("notification_ids")]
        public string[] NotificationIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Account> builder)
        {

        }
    }
}
