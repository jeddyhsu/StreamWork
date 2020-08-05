using StreamWork.DataModels;

namespace StreamWork.ViewModels.Stream
{
    public class StreamPageViewModel
    {
        public Profile StudentUserProfile { get; set; }
        public Profile TutorStreamingUserProfile { get; set; }
        public Profile TutorUserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public bool IsFollowing { get; set; }
    }
}