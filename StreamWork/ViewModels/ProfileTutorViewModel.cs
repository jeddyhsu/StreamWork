using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels
{
    public class ProfileTutorViewModel
    {
        public UserProfile userProfile { get; set; }
        public List<UserChannel> userChannels { get; set; }
        public List<UserArchivedStreams> userArchivedVideos { get; set; }
        public List<UserLogin> userLogins { get; set; }
    }
}
