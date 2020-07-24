using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<UserLogin> PopularStreamTutors { get; set; }
        public List<UserChannel> StreamResults { get; set; }
        public List<UserArchivedStreams> ArchiveResults { get; set; }
        public UserLogin UserProfile { get; set; }

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
            PopularStreamTutors = await storage.GetList<UserLogin>(SQLQueries.GetApprovedTutorsByFollowers);
            StreamResults = await search.SearchChannels(subject, searchQuery);
            ArchiveResults = await search.SearchArchivedStreams(subject, searchQuery);
            UserProfile = await session.GetCurrentUser();
            Subject = string.IsNullOrEmpty(subject) ? "All subjects" : subject;
            SearchQuery = searchQuery;
            SubjectIcon = subjectService.GetSubjectIcon(subject);
        }
    }
}
