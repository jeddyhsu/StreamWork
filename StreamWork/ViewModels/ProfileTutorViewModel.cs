using System.Collections;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.TutorObjects;

namespace StreamWork.ViewModels {

    public class ProfileTutorViewModel {

        public UserLogin UserProfile { get; set; }
        public UserLogin StudentOrTutorProfile{ get; set; } //just in case
        public List<UserChannel> UserChannels { get; set; }
        public List<UserArchivedStreams> UserArchivedVideos { get; set; }
        public List<UserLogin> UserLogins { get; set; }

        public int NumberOfStreams { get; set; }
        public string Subject { get; set; }
        
        public string ChatSecretKey { get; set; }
        public bool IsUserFollowingThisTutor { get; set; }

        public List<Day> Schedule = new List<Day>();
    }
}