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

        public UserLogin CurrentUserProfile { get; set; }
        public List<UserArchivedStreams> Videos { get; set; }
        public List<UserLogin> PopularTutors { get; set; }
        public List<UserChannel> LiveChannels { get; set; }
        public List<UserLogin> AllTutors { get; set; }

        public BrowseModel(CookieService cookie, StorageService storage)
        {
            cookieService = cookie;
            storageService = storage;
        }

        public async Task<IActionResult> OnGet()
        {
            CurrentUserProfile = await cookieService.GetCurrentUser();


            var tutors = await storageService.GetList<UserLogin>(SQLQueries.GetAllApprovedTutors, "");

            Videos = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetAllArchivedStreams, "");
            PopularTutors = tutors.GetRange(0,5);
            LiveChannels = await storageService.GetList<UserChannel>(SQLQueries.GetAllUserChannelsThatAreStreaming, "");
            AllTutors = tutors;

            return Page();
        }
    }
}
