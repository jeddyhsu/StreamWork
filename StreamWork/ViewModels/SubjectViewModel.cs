using System;
using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class SubjectViewModel
    {
        public List<UserLogin> UserLogins { get; set; }
        public List<UserChannel> UserChannels { get; set; }
        public UserLogin UserProfile { get; set; }

        public string Subject { get; set; }
        public string SubjectIcon { get; set; }
    }
}
