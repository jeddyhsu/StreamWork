﻿using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class ChatViewModel
    {
        public UserLogin UserProfile { get; set; }
        public string ChatId;
        public List<Chats> Chats { get; set; }
        public string ChatColor { get; set; }
        public string TutorUserName { get; set; }
    }
}
