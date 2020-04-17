using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class ProfileStudentViewModel
    {
        public UserLogin UserProfile { get; set; }
        public List<UserLogin> AllTutors { get; set; }
        public List<UserLogin> FollowedTutors { get; set; }
        public List<UserChannel> LiveChannels { get; set; }
        public List<UserArchivedStreams> ArchivedStreams {get; set; }
        public List<UserArchivedStreams> PreviouslyWatchedStreams { get; set; }
    }
}
