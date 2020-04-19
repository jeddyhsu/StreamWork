using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class PopularStreamTutorsViewModel
    {
        public UserLogin GenericUserProfile { get; set; }
        public List<UserLogin> PopularStreamTutors { get; set; }
    }
}
