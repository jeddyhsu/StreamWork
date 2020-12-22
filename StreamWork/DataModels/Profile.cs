using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;
using StreamWork.HelperMethods;

namespace StreamWork.DataModels
{
    public class Profile : StorageBase
    {
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

        public Profile(User user, string firstName, string lastName) : this(user.Id, firstName, lastName) { }

        public Profile (string userId, string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Caption = null;
            Description = null;
            Location = null;
            Color = MiscHelperMethods.GetRandomColor();
            Picture = MiscHelperMethods.defaultProfilePicture;
            Banner = MiscHelperMethods.defaultBanner;
            LinkedInUrl = null;
            InstagramUrl = null;
            FacebookUrl = null;
            TwitterUrl = null;
            FollowIds = new string[] { };
            FollowerIds = new string[] { }; // TODO Add team as followers
        }
    }
}
