using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Tutor
{
    public class TutorStream : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly StreamService streamService;
        private readonly NotificationService notificationService;

        public UserLogin CurrentUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public TutorStream(StorageService storage, CookieService cookie, StreamService stream, NotificationService notification)
        {
            storageService = storage;
            cookieService = cookie;
            streamService = stream;
            notificationService = notification;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!cookieService.Authenticated || (await cookieService.GetCurrentUser()).ProfileType != "tutor")
            {
                return Redirect(cookieService.Url("/Home/SignIn"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, new string[] { CurrentUserProfile.Username });
            ChatInfo = "1234";

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);

            return Page();
        }

        public IActionResult OnPostIsLive(string channelKey)
        {
            if (streamService.IsLive(channelKey)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostRegisterStream()
        {
            var userProfile = await cookieService.GetCurrentUser();
            var userChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, userProfile.Username);

            if (streamService.StartStream(Request, userProfile, userChannel)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
