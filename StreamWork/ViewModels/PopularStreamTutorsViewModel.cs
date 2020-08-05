using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels
{
    public class PopularStreamTutorsViewModel
    {
        public DataModels.Profile GenericUserProfile { get; set; }
        public List<DataModels.Profile> PopularStreamTutors { get; set; }
    }
}
