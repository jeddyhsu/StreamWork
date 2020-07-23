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
        private readonly SessionService sessionService;
        private readonly StorageService storageService;
        private readonly StreamService streamService;
        private readonly NotificationService notificationService;

        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public TutorStream(StorageService storage, SessionService session, StreamService stream, NotificationService notification)
        {
            storageService = storage;
            sessionService = session;
            streamService = stream;
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
            ChatInfo = "1234";

            Notifications = await notificationService.GetNotifications(UserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(UserProfile.Username);

            return Page();
        }

        public IActionResult OnPostIsLive(string channelKey)
        {
            if (streamService.IsLive(channelKey)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostRegisterStream()
        {
            var userProfile = await sessionService.GetCurrentUser();
            var userChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, userProfile.Username);

            if (streamService.StartStream(Request, userProfile, userChannel)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
