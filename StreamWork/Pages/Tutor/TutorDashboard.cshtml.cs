using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Tutor
{
    public class TutorDashboard : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly SearchService searchService;
        private readonly NotificationService notificationService;
        private readonly ChatService chatService;

        public Profile CurrentUserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfFollowees { get; set; }
        public int NumberOfViews { get; set; }
        public List<Video> UserArchivedStreams { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Schedule> Schedule { get; set; }
        public List<Profile> Followers { get; set; }
        public List<Profile> Followees { get; set; }
        public List<string> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public string DefaultBanner { get; set; }
        public string DefaultProfilePicture { get; set; }

        public TutorDashboard(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow, SearchService search, NotificationService notification, ChatService chat)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            searchService = search;
            notificationService = notification;
            chatService = chat;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!cookieService.Authenticated || (await cookieService.GetCurrentUser()).ProfileType != "tutor")
            {
                return Redirect(cookieService.Url("/Home/SignIn/SW"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, new string[] { CurrentUserProfile.Username });

            UserArchivedStreams = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { CurrentUserProfile.Username });
            Sections = profileService.GetSections(CurrentUserProfile);
            Topics = profileService.GetTopics(CurrentUserProfile);
            Schedule = await scheduleService.GetSchedule(CurrentUserProfile);

            Followers = await followService.GetAllFollowers(CurrentUserProfile.Id);
            Followees = await followService.GetAllFollowees(CurrentUserProfile.Id);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);
            NumberOfFollowers = Followers == null ? 0 : Followers.Count;
            NumberOfFollowees = Followees == null ? 0 : Followees.Count;

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            DefaultBanner = MiscHelperMethods.defaultBanner;
            DefaultProfilePicture = MiscHelperMethods.defaultProfilePicture;

            return Page();
        }

        public async Task<IActionResult> OnPostSaveScheduleTask()
        {
            var userProfile = await cookieService.GetCurrentUser();
            var sortedList = await scheduleService.SaveToSchedule(Request, userProfile.Username);

            if (sortedList != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Sorted = sortedList });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteScheduleTask(string taskId)
        {
            var userProfile = await cookieService.GetCurrentUser();
            var sortedList = await scheduleService.DeleteFromSchedule(taskId, userProfile);

            if (sortedList != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Sorted = sortedList });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveEditedVideo()
        {
            var savedInfo = await profileService.SaveEditedArchivedStream(Request);

            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteVideo(string id)
        {
            await chatService.DeleteAllChatsWithArchiveVideoId(id);
            var savedInfo = await profileService.DeleteStream(id);

            if (savedInfo) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSearchVideos(string username)
        {
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { username }) });
        }
    }
}
