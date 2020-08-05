using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels.Student
{
    public class ProfileStudentViewModel
    {
        public Profile StudentUserProfile { get; set; }
        public List<Profile> NonFollowedTutors { get; set; }
        public List<Profile> FollowedTutors { get; set; }
        public List<Channel> LiveChannels { get; set; }
        public List<Video> ArchivedStreams {get; set; }
        public List<Video> PreviouslyWatchedStreams { get; set; }
    }
}
