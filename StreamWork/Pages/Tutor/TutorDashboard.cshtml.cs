using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;
using StreamWork.ViewModels;

namespace StreamWork.Pages.Tutor
{
    public class TutorDashboard : PageModel
    {
        private readonly SessionService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly EditService editService;
        private readonly SearchService searchService;
        private readonly NotificationService notificationService;

        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Schedule> Schedule { get; set; }
        public List<UserLogin> Followers { get; set; }
        public List<UserLogin> Followees { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public SearchViewModel SearchViewModel { get; set; }

        public TutorDashboard(StorageService storage, SessionService session, ProfileService profile, ScheduleService schedule, FollowService follow, EditService edit, SearchService search, NotificationService notification)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            editService = edit;
            searchService = search;
            notificationService = notification;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!sessionService.Authenticated)
            {
                //return Redirect(session.Url("/Home/Login?dest=-Tutor-TutorDashboard"));
            }

            UserProfile = await sessionService.GetCurrentUser();
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, new string[] { UserProfile.Username });

            UserArchivedStreams = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { UserProfile.Username });
            Sections = profileService.GetSections(UserProfile);
            Topics = profileService.GetTopics(UserProfile);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);

            Followers = await followService.GetAllFollowers(UserProfile.Id);
            Followees = await followService.GetAllFollowees(UserProfile.Id);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);
            NumberOfFollowers = Followers == null ? 0 : Followers.Count;

            Notifications = await notificationService.GetNotifications(UserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(UserProfile.Username);

            return Page();
        }

        public async Task<IActionResult> OnPostSaveScheduleTask()
        {
            var userProfile = await sessionService.GetCurrentUser();
            var sortedList = await scheduleService.SaveToSchedule(Request, userProfile.Username);

            if (sortedList != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Sorted = sortedList });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteScheduleTask(string taskId)
        {
            var userProfile = await sessionService.GetCurrentUser();
            var sortedList = await scheduleService.DeleteFromSchedule(taskId, userProfile.Username);

            if (sortedList != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Sorted = sortedList });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveEditedStream()
        {
            var savedInfo = await profileService.SaveEditedArchivedStream(Request);

            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteStream(string id)
        {
            var savedInfo = await profileService.DeleteStream(id);

            if (savedInfo) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSearchArchivedStreams(string searchTerm, string filter)
        {
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = await searchService.SearchArchivedStreams(filter, searchTerm) });
        }
    }
}
