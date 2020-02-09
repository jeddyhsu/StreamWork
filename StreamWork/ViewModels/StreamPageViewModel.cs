using System;
using StreamWork.DataModels;

namespace StreamWork.ViewModels {
    public class StreamPageViewModel {

        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatBox { get; set; }
        public bool IsUserFollowingThisTutor { get; set; }
    }
}