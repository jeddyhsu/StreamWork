using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class User : IStorageBase<User>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        [JsonPropertyName("profile_id")]
        public string ProfileId { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<User> builder)
        {

        }
    }
}
