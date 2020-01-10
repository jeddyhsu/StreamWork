using System;
using StreamWork.DataModels;

namespace StreamWork.ViewModels {
    public class StreamPageViewModel {

        public UserLogin UserProfile { get; set; }
        public ProfileTutorViewModel Profile { get; set; }
        public string[] UrlParams { get; set; }
        public string StreamName { get; set; }

        public bool IsSubscribed () {
            if (DateTime.UtcNow.CompareTo(Profile.UserProfile.Expiration) < 0 || (Profile.UserProfile.ProfileType.Equals("tutor") && Profile.UserProfile.AcceptedTutor)) {
                return true;
            }
            return false;
        }
    }
}