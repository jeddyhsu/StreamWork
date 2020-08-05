using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class ProfileViewModel
    {
        public Profile TutorUserProfile { get; set; }
        public Profile GenericUserProfile { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public bool IsFollowing { get; set; }
        public List<Video> UserArchivedStreams { get; set; }
        public List<Schedule> Schedule { get; set; }
    }
}
