using System;
using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Profiles : StorageBase
    {
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
        public string InstagramUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string ProfileColor { get; set; }
      
        //Students
        public decimal Balance { get; set; }
        public DateTime Expiration { get; set; }
        public bool TrialAccepted { get; set; }
        public string PayPalAddress { get; set; }

        //Tutors
        public bool AcceptedTutor { get; set; }

        public DateTime LastLogin { get; set; }
        public DateTime ProfileSince { get; set; }
    }
}
