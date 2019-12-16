using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels {

    public class ProfileTutorViewModel {

        public UserLogin userProfile { get; set; }
        public UserLogin studentOrtutorProfile{ get; set; } //just in case
        public List<UserChannel> userChannels { get; set; }
        public List<UserArchivedStreams> userArchivedVideos { get; set; }
        public List<UserLogin> userLogins { get; set; }

        public string ChatSecretKey { get; set; }

    }
}