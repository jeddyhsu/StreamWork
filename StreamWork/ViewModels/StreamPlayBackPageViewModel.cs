using System;
using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class StreamPlayBackPageViewModel
    {
        public UserLogin GenericUserProfile { get; set; }
        public UserLogin TutorUserProfile { get; set; }
        public UserArchivedStreams ArchivedStream { get; set; }
        public UserChannel UserChannel { get; set; }
        public List<UserArchivedStreams> ArchivedStreams { get; set; }
        public int NumberOfStreams { get; set; }
        public bool IsFollowing { get; set; }

        //public bool IsSubscribed()
        //{
        //    if (DateTime.UtcNow.CompareTo(StudentUserProfile.Expiration) < 0 || (StudentUserProfile.ProfileType.Equals("tutor") && StudentUserProfile.AcceptedTutor)) return true;

        //    return false;
        //}
    }
}
