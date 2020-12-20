using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.DataModels.Joins;
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
        public List<TutorSubject> PopularTutors { get; set; }
        public List <Channel> LiveChannels { get; set; }
        public List<ScheduledStream> AllScheduledStreams { get; set; }
        public List<TutorSubject> AllTutors { get; set; }
        public List<string> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public string SearchTerm { get; set; }
        public bool isFilterTerm { get; set; }

        public BrowseModel(CookieService cookie, StorageService storage, NotificationService notification, SearchService search)
        {
            cookieService = cookie;
            storageService = storage;
            notificationService = notification;
            searchService = search;
        }

        public async Task<IActionResult> OnGet(string searchTerm)
        {
            //var tutors = await storageService.GetList<TutorSubject>(SQLQueries.GetApprovedTutorSubjects, "");

            CurrentUserProfile = await cookieService.GetCurrentUser();
            //Videos = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsInDescendingOrderByViews, "");
            //PopularTutors = tutors.GetRange(0,3);
            //LiveChannels = await storageService.GetList<Channel>(SQLQueries.GetAllUserChannelsThatAreStreaming, "");
            AllTutors = new List<TutorSubject>();
            Hashtable table = new Hashtable();
            //foreach (var tutor in tutors)
            //{
            //    if (!table.Contains(tutor.Username))
            //    {
            //        table.Add(tutor.Username, tutor);
            //        AllTutors.Add(tutor);
            //    }
            //}

            //AllScheduledStreams = await storageService.GetList<Schedule>(SQLQueries.GetAllScheduledStreams, "");

            if(searchTerm != "SW")
            {
                SearchTerm = searchTerm;
                if(MiscHelperMethods.GetCorrespondingStreamColor(SearchTerm) != null)
                {
                    isFilterTerm = true;
                }
            }

            //if(CurrentUserProfile != null)
            //{
            //    Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            //    AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            //}

            return Page();
        }

        //public async Task<IActionResult> OnPostSearchStreams()
        //{
        //    //return new JsonResult(new { Message = JsonResponse.Success.ToString(), Channels = await storageService.GetList<Channel>(SQLQueries.GetAllUserChannelsThatAreStreaming, ""), Videos = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsInDescendingOrderByViews, "") });
        //}

        //public async Task<IActionResult> OnPostSearchSchedule()
        //{
        //    //return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = await storageService.GetList<Schedule>(SQLQueries.GetAllScheduledStreams, "") });
        //}

        //public async Task<IActionResult> OnPostSearchTutors(string filter, string searchTerm)
        //{
        //    //return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = await storageService.GetList<TutorSubject>(SQLQueries.GetApprovedTutorSubjects, "") });
        //}
    }
}
