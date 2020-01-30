using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels {

    public class ProfileTutorViewModel {

        public UserLogin userProfile { get; set; }
        public UserLogin userProfile2 { get; set; } //just in case
        public List<UserChannel> userChannels { get; set; }
        public List<UserArchivedStreams> userArchivedVideos { get; set; }
        public List<UserLogin> userLogins { get; set; }

    }
}