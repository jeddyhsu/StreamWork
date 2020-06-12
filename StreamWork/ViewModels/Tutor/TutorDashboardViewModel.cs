using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.TutorObjects;

namespace StreamWork.ViewModels {

    public class TutorDashboardViewModel {

        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }

        public List<UserArchivedStreams> UserArchivedStreams { get; set; }

        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }

        public List<Recommendation> Recommendations { get; set; }
        public List<Day> Schedule = new List<Day>();

        public SearchViewModel SearchViewModel { get; set; }
    }
}