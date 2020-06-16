using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class UserLogin : IStorageBase<UserLogin>
    {
        [Key]
        //Both
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ChangePasswordKey { get; set; }
        public string ProfileType { get; set; }
        public string ProfileCaption { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfileParagraph { get; set; }
        public string College { get; set; }
        public string NotificationSubscribe { get; set; }
        public string Location { get; set; }
        public string LinkedInUrl { get; set; }
        public string TimeZone { get; set; }
        public string ProfileBanner { get; set; }

        //Students
        public decimal Balance { get; set; }
        public DateTime Expiration { get; set; }
        public bool TrialAccepted { get; set; }
        public string PayPalAddress { get; set; }

        //Tutors
        public bool AcceptedTutor { get; set; }

        public DateTime LastLogin { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<UserLogin> builder)
        {

        }
    }
}
