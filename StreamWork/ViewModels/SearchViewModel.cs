using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ViewModels {
    public class SearchViewModel {
        public List<UserLogin> PopularStreamTutors { get; set; }
        public List<UserChannel> StreamResults { get; set; }
        public List<UserArchivedStreams> ArchiveResults { get; set; }
        public UserLogin UserProfile { get; set; }

        public string Subject { get; set; }
        public string SearchQuery { get; set; }
        public string SubjectIcon { get; set; }
    }
}
