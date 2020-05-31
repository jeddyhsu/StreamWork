using StreamWork.DataModels;

namespace StreamWork.ViewModels {
    public class StreamPageViewModel {

        public UserLogin StudentUserProfile { get; set; }
        public UserLogin TutorStreamingUserProfile { get; set; }
        public UserLogin TutorUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public bool IsUserFollowingThisTutor { get; set; }
    }
}