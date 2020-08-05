using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class SearchModel : PageModel
    {
        private readonly StorageService storage;
        private readonly CookieService session;
        private readonly SearchService search;
        private readonly SubjectService subjectService;

        public List<Profile> PopularStreamTutors { get; set; }
        public List<Channel> StreamResults { get; set; }
        public List<Video> ArchiveResults { get; set; }
        public Profile UserProfile { get; set; }

        public string Subject { get; set; }
        public string SearchQuery { get; set; }
        public string SubjectIcon { get; set; }

        public SearchModel(StorageService storage, CookieService session, SearchService search, SubjectService subjectService)
        {
            this.storage = storage;
            this.session = session;
            this.search = search;
            this.subjectService = subjectService;
        }

        public async Task OnGet([FromQuery(Name = "s")] string subject, [FromQuery(Name = "q")] string searchQuery)
        {
            PopularStreamTutors = await storage.GetList<Profile>(SQLQueries.GetApprovedTutorsByFollowers);
            StreamResults = await search.SearchChannels(subject, searchQuery);
            ArchiveResults = await search.SearchVideos(subject, searchQuery);
            UserProfile = await session.GetCurrentUser();
            Subject = string.IsNullOrEmpty(subject) ? "All subjects" : subject;
            SearchQuery = searchQuery;
            SubjectIcon = subjectService.GetSubjectIcon(subject);
        }
    }
}
