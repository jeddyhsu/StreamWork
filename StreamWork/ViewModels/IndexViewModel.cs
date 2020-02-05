using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class IndexViewModel
    {
        public UserChannel UserChannel { get; set; }
        public UserLogin UserProfile { get; set; }
        public UserArchivedStreams UserArchivedStream { get; set; }
        public UserLogin UserLogin { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
    }
}
