using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Student
{
    public class StudentDashboard : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;

        public UserLogin CurrentUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public List<UserLogin> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Comment> Comments { get; set; }

        public StudentDashboard(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!cookieService.Authenticated)
            {
                return Redirect(cookieService.Url("/Home/SignIn"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();

            RelatedTutors = (await storageService.GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, new string[] { CurrentUserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(CurrentUserProfile);
            Topics = profileService.GetTopics(CurrentUserProfile);

            return Page();
        }
    }
}
