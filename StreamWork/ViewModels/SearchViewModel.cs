using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels {
    public class SearchViewModel {
        public List<Profile> PopularStreamTutors { get; set; }
        public List<Channel> StreamResults { get; set; }
        public List<Video> ArchiveResults { get; set; }
        public Profile UserProfile { get; set; }

        public string Subject { get; set; }
        public string SearchQuery { get; set; }
        public string SubjectIcon { get; set; }
    }
}
