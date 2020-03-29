using System;
using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class StreamPlayBackPageViewModel
    {
        public UserLogin UserProfile { get; set; }
        public UserLogin TutorUserProfile { get; set; }
        public UserArchivedStreams ArchivedStream { get; set; }
        public UserChannel UserChannel { get; set; }
        public List<UserArchivedStreams> ArchivedStreams { get; set; }
        public int NumberOfStreams { get; set; }
        public bool IsUserFollowingThisTutor { get; set; }

        public bool IsSubscribed()
        {
            if (DateTime.UtcNow.CompareTo(UserProfile.Expiration) < 0 || (UserProfile.ProfileType.Equals("tutor") && UserProfile.AcceptedTutor)) return true;

            return false;
        }
    }
}
