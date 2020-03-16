using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class PopularStreamTutorsViewModel
    {
        public UserLogin UserProfile { get; set; }
        public List<UserLogin> PopularStreamTutors { get; set; }
    }
}
