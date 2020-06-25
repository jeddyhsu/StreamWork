using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;
using StreamWork.TutorObjects;

namespace StreamWork.ViewModels
{
    public class TutorDashboardTestModel : PageModel
    {
        private readonly SessionService session;
        private readonly StorageService storage;

        public UserLogin UserLogin { get; private set; }
        public UserChannel Channel { get; private set; }
        public List<UserArchivedStreams> ArchivedStreams { get; private set; }
        public List<Section> Sections { get; private set; }
        public List<Topic> Topics { get; private set; }
        public int TotalViews { get; private set; }
        public Schedule TutorSchedule { get; private set; }

        public TutorDashboardTestModel(StorageService storage, SessionService session)
        {
            this.storage = storage;
            this.session = session;
        }

        public IActionResult OnGet()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(session.Url("/Home/Login?dest=-Tutor-TutorDashboard")); // <-- We should store the page to redirect to in the session, instead of the parameters
            }

            User = session.CurrentUser;
            //Channel = storage.GetChannel(User.Id);

            //ArchivedStreams = storage.GetArchivedStreamsByTutor(User.Id);
            //Sections = storage.GetSectionsByTutor(User.Id);
            //Topics = storage.GetTopicsByTutor(User.Id);
            //Recommendations = storage.GetRecommendationsToTutor(User.Id);
            //TutorSchedule = storage.GetTutorSchedule(User.Id);

            //TotalViews = ArchivedStreams.Sum(x => x.Views);
            //FollowerCount = storage.GetFollowerCountOf(User.Id);

            return Page();
        }
    }
}
