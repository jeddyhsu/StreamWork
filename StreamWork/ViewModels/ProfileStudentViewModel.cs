using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class ProfileStudentViewModel
    {
        public UserLogin userProfile { get; set; }
        public List<UserLogin> userLogins { get; set; }
        public List<UserChannel> userChannels { get; set; }
        public List<UserArchivedStreams> userArchivedStreams {get; set; }
    }
}
