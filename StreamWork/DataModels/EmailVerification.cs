using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class EmailVerification : IStorageBase<EmailVerification>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("verification_code")]
        public string VerificationCode { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<EmailVerification> builder)
        {

        }
    }
}
