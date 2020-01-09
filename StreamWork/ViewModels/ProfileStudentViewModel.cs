using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class ProfileStudentViewModel
    {
        public UserLogin UserProfile { get; set; }
        public List<UserLogin> UserLogins { get; set; }
        public List<UserChannel> UserChannels { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams {get; set; }
    }
}
