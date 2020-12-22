using System;
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

        public User (out Account account, out Profile profile, string emailAddress, string password, string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString();
            EmailAddress = emailAddress;
            Password = password; // Should already be encrypted
            account = new Account();
            AccountId = account.Id;
            profile = new Profile(Id, firstName, lastName);
            ProfileId = profile.Id;
            ChannelId = null;
        }
    }
}
