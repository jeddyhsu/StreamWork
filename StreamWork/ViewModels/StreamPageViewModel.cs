using System;
using StreamWork.DataModels;

namespace StreamWork.ViewModels {
    public class StreamPageViewModel {
        public UserLogin userProfile { get; set; }
        public ProfileTutorViewModel profile { get; set; }
        public string[] urlParams { get; set; }
        public string streamName { get; set; }

        public bool IsSubscribed () {
            if (DateTime.UtcNow.CompareTo(profile.UserProfile.Expiration) < 0 || (profile.UserProfile.ProfileType.Equals("tutor") && profile.UserProfile.AcceptedTutor)) {
                return true;
            }
            return false;
        }
    }
}