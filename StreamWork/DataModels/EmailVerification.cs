using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class EmailVerification : IStorageBase<EmailVerification>
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("email_address")]
        public string EmailAddress { get; set; }

        [BsonElement("verification_code")]
        public string VerificationCode { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<EmailVerification> builder)
        {

        }
    }
}
