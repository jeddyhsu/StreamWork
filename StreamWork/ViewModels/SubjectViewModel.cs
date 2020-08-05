using System;
using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class SubjectViewModel
    {
        public List<Profile> UserLogins { get; set; }
        public List<Channel> UserChannels { get; set; }
        public Profile UserProfile { get; set; }

        public string Subject { get; set; }
        public string SubjectIcon { get; set; }
    }
}
