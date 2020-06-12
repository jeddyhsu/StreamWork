using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.TutorObjects;

namespace StreamWork.ViewModels
{
    public class ProfileViewModel
    {
        public UserLogin TutorUserProfile { get; set; }
        public UserLogin GenericUserProfile { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public bool IsFollowing { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<Day> Schedule { get; set; }
    }
}
