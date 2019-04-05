using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels
{
    public class ProfileTutorViewModel
    {
        public UserProfile userProfile { get; set; }
        public List<YoutubeChannelID> youtubeStreamKeys { get; set; }
        public List<ArchivedStreams> archivedVideos { get; set; }
    }
}
