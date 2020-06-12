using StreamWork.DataModels;

namespace StreamWork.ViewModels.Stream
{
    public class StreamPageViewModel
    {
        public UserLogin StudentUserProfile { get; set; }
        public UserLogin TutorStreamingUserProfile { get; set; }
        public UserLogin TutorUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public bool IsFollowing { get; set; }
    }
}