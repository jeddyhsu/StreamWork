using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.TutorObjects;

namespace StreamWork.ViewModels {

    public class ProfileTutorViewModel {

        public UserLogin TutorUserProfile { get; set; }
        public UserLogin GenericUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }

        public List<UserChannel> UserChannels { get; set; }
        public List<UserArchivedStreams> UserArchivedVideos { get; set; }
        public List<UserLogin> UserProfiles { get; set; }

        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        
        public string ChatSecretKey { get; set; }
        public bool IsUserFollowingThisTutor { get; set; }

        public List<Recommendation> Recommendations { get; set; }
        public List<Day> Schedule = new List<Day>();

        public SearchViewModel SearchViewModel { get; set; }
    }
}