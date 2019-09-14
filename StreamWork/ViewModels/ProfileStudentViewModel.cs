using System.Collections.Generic;
using StreamWork.Models;

namespace StreamWork.ViewModels
{
    public class ProfileStudentViewModel
    {
        public UserProfile userProfile { get; set; }
        public List<UserLogin> userLogins { get; set; }
    }
}
