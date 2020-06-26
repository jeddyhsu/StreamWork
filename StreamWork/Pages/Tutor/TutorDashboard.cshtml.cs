using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;
using StreamWork.TutorObjects;
using StreamWork.ViewModels;

namespace StreamWork.Pages.Tutor
{
    public class TutorDashboard : PageModel
    {
        private readonly SessionService session;
        private readonly StorageService storage;

        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Schedule> Schedule { get; set; }

        public SearchViewModel SearchViewModel { get; set; }

        public TutorDashboard(StorageService storage, SessionService session)
        {
            this.storage = storage;
            this.session = session;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!session.Authenticated)
            {
                //return Redirect(session.Url("/Home/Login?dest=-Tutor-TutorDashboard"));
            }

            UserProfile = await session.GetCurrentUser();
            UserChannel = await storage.GetChannel(UserProfile.Username);

            UserArchivedStreams = await storage.GetArchivedStreamsByTutor(UserProfile.Username);
            Sections = storage.GetSectionsByTutor(UserProfile.Username);
            Topics = storage.GetTopicsByTutor(UserProfile.Username);
            Comments = storage.GetCommentsToTutor(UserProfile.Username);
            Schedule = storage.GetSchedule(UserProfile.Username);

            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);
            NumberOfFollowers = storage.GetFollowerCountOf(UserProfile.Username);

            return Page();
        }
    }
}
