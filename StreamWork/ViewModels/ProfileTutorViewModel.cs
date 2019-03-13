using System;
using System.Collections.Generic;
using StreamWork.Models;

namespace StreamWork.ViewModels
{
    public class ProfileTutorViewModel
    {
        public UserProfile userProfile { get; set; }
        public List<YoutubeStreamKeys> youtubeStreamKeys { get; set; }
    }
}
