using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class ProfileStudentViewModel
    {
        public UserLogin StudentUserProfile { get; set; }
        public List<UserLogin> NonFollowedTutors { get; set; }
        public List<UserLogin> FollowedTutors { get; set; }
        public List<UserChannel> LiveChannels { get; set; }
        public List<UserArchivedStreams> ArchivedStreams {get; set; }
        public List<UserArchivedStreams> PreviouslyWatchedStreams { get; set; }
    }
}
