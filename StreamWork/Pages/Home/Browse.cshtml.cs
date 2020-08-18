using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class BrowseModel : PageModel
    {
        private readonly StorageService storageService;
        private readonly CookieService cookieService;
        private readonly NotificationService notificationService;
        private readonly SearchService searchService;

        public Profile CurrentUserProfile { get; set; }
        public List<Video> Videos { get; set; }
        public List<Profile> PopularTutors { get; set; }
        public List <Channel> LiveChannels { get; set; }
        public List<Schedule> AllScheduledStreams { get; set; }
        public List<Profile> AllTutors { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public BrowseModel(CookieService cookie, StorageService storage, NotificationService notification, SearchService search)
        {
            cookieService = cookie;
            storageService = storage;
            notificationService = notification;
            searchService = search;
        }

        public async Task<IActionResult> OnGet()
        {
            CurrentUserProfile = await cookieService.GetCurrentUser();

            var tutors = await storageService.GetList<Profile>(SQLQueries.GetAllApprovedTutors, "");

            Videos = await storageService.GetList<Video>(SQLQueries.GetAllArchivedStreams, "");
            PopularTutors = tutors.GetRange(0,5);
            LiveChannels = await storageService.GetList<Channel>(SQLQueries.GetAllUserChannelsThatAreStreaming, "");
            AllTutors = tutors;
            AllScheduledStreams = await storageService.GetList<Schedule>(SQLQueries.GetAllScheduledStreams, "");

            if(CurrentUserProfile != null)
            {
                Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
                AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSearchStreams(string filter, string searchTerm)
        {
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Channels = await searchService.SearchChannels(filter, searchTerm), Videos = await searchService.SearchVideos(filter, searchTerm) });
        }

        public async Task<IActionResult> OnPostSearchSchedule(string filter, string searchTerm)
        {
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = await searchService.SearchSchedule(filter, searchTerm) });
        }

        public async Task<IActionResult> OnPostSearchTutors(string filter, string searchTerm)
        {
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = await searchService.SearchTutors(filter, searchTerm) });
        }
    }
}
