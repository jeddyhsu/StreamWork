using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels
{
    public class ProfileViewModel
    {
        public List<StreamWorkLogin> users { get; set; }
        public List<ArchivedStreams> archivedStreams { get; set; }
    }
}
