using System;
using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels.Stream
{
    public class StreamPlayBackPageViewModel
    {
        public Profile GenericUserProfile { get; set; }
        public Profile TutorUserProfile { get; set; }
        public Video ArchivedStream { get; set; }
        public Channel UserChannel { get; set; }
        public List<Video> ArchivedStreams { get; set; }
        public int NumberOfStreams { get; set; }
        public bool IsFollowing { get; set; }

        //public bool IsSubscribed()
        //{
        //    if (DateTime.UtcNow.CompareTo(StudentUserProfile.Expiration) < 0 || (StudentUserProfile.ProfileType.Equals("tutor") && StudentUserProfile.AcceptedTutor)) return true;

        //    return false;
        //}
    }
}
