using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class IndexViewModel
    {
        public Channel UserChannel { get; set; }
        public Profile GenericUserProfile { get; set; }
        public Video UserArchivedStream { get; set; }
        public Profile UserLogin { get; set; }
        public List<Video> UserArchivedStreams { get; set; }
        public bool IsUserFollowingThisTutor { get; set; }
        public string ChatInfo { get; set; }
    }
}
