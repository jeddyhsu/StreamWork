using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Profile : IStorageBase<Profile>
    {
        
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Color { get; set; }
        public string Picture { get; set; }
        public string Banner { get; set; }
        public string LinkedInUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string[] Follows { get; set; }
        public string[] Followers { get; set; }
        public string[] TopicFollows { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Profile> builder)
        {

        }
    }
}
