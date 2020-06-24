using System;
namespace StreamWork.Hubs
{
    public class ChatInformation
    {
        public string ChatId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime Date { get; set; }
        public string ChatColor { get; set; }
        public int TimeOffset { get; set; }
        public long QuestionCount { get; set; }
    }
}
