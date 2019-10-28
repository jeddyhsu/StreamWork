using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels
{
    public class ProfileStudentViewModel
    {
        public UserProfile userProfile { get; set; }
        public List<UserLogin> userLogins { get; set; }
        public List<UserChannel> userChannels { get; set; }
        public List<UserArchivedStreams> userArchivedStreams {get; set; }
    }
}
