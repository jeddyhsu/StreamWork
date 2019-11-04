using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels {

    public class StreamPageViewModel {

        public ProfileTutorViewModel profile { get; set; }
        public string[] urlParams { get; set; }

        public bool IsSubscribed () {
            if (DateTime.UtcNow.CompareTo(profile.userProfile.Expiration) < 0 || (profile.userProfile.ProfileType.Equals("tutor") && profile.userProfile.AcceptedTutor)) {
                return true;
            }
            return false;
        }
    }
}