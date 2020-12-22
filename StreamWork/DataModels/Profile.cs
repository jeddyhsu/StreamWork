using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Profile : IStorageBase<Profile>
    {
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("first_name")]
        public string FirstName { get; set; }

        [BsonElement("last_name")]
        public string LastName { get; set; }

        [BsonElement("caption")]
        public string Caption { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("picture")]
        public string Picture { get; set; }

        [BsonElement("banner")]
        public string Banner { get; set; }

        [BsonElement("linkedin_url")]
        public string LinkedInUrl { get; set; }

        [BsonElement("instagram_url")]
        public string InstagramUrl { get; set; }

        [BsonElement("facebook_url")]
        public string FacebookUrl { get; set; }

        [BsonElement("twitter_url")]
        public string TwitterUrl { get; set; }

        [BsonElement("follow_ids")]
        public string[] FollowIds { get; set; }

        [BsonElement("follower_ids")]
        public string[] FollowerIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Profile> builder)
        {

        }
    }
}
