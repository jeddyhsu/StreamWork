using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class TutorStreamViewModel
    {
        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
    }
}
