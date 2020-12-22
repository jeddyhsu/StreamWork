using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class User : StorageBase
    {
        [BsonElement("email_address")]
        public string EmailAddress { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("account_id")]
        public string AccountId { get; set; }

        [BsonElement("profile_id")]
        public string ProfileId { get; set; }

        [BsonElement("channel_id")]
        public string ChannelId { get; set; }
    }
}
