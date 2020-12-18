using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Profile : IStorageBase<Profile>
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("caption")]
        public string Caption { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonPropertyName("linkedin_url")]
        public string LinkedInUrl { get; set; }

        [JsonPropertyName("instagram_url")]
        public string InstagramUrl { get; set; }

        [JsonPropertyName("facebook_url")]
        public string FacebookUrl { get; set; }

        [JsonPropertyName("twitter_url")]
        public string TwitterUrl { get; set; }

        [JsonPropertyName("follow_ids")]
        public string[] FollowIds { get; set; }

        [JsonPropertyName("follower_ids")]
        public string[] FollowerIds { get; set; }

        [Timestamp] [JsonIgnore]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Profile> builder)
        {

        }
    }
}
