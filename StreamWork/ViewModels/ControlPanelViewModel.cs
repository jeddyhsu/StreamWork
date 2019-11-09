using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.ViewModels {
    public class ControlPanelViewModel {
        public List<UserLogin> Students { get; set; }
        public List<UserLogin> Tutors { get; set; }
        public List<DonationAttempt> DonationAttempts { get; set; }
    }
}
