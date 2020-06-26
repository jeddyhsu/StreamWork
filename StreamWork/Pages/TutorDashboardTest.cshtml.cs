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
        public List<Comment> Comments { get; private set; }
        public List<Schedule> TutorSchedule { get; private set; }
        public int TotalViews { get; private set; }
        public int FollowerCount { get; private set; }

        public TutorDashboardTestModel(StorageService storage, SessionService session)
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

            UserLogin = await session.GetCurrentUser();
            Channel = await storage.GetChannel(UserLogin.Username);

            ArchivedStreams = await storage.GetArchivedStreamsByTutor(UserLogin.Username);
            Sections = storage.GetSectionsByTutor(UserLogin.Username);
            Topics = storage.GetTopicsByTutor(UserLogin.Username);
            Comments = storage.GetCommentsToTutor(UserLogin.Username);
            TutorSchedule = storage.GetSchedule(UserLogin.Username);

            TotalViews = ArchivedStreams.Sum(x => x.Views);
            FollowerCount = storage.GetFollowerCountOf(UserLogin.Username);

            return Page();
        }
    }
}
