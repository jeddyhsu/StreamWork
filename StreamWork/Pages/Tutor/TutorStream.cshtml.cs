using System;
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
        public EncryptionService encryptionService;

        public Profile CurrentUserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public Schedule ScheduledStream { get; set; }
        
        public TutorStream(StorageService storage, CookieService cookie, StreamService stream, NotificationService notification, EncryptionService encryption)
        {
            storageService = storage;
            cookieService = cookie;
            streamService = stream;
            notificationService = notification;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string scheduleId)
        {
            if (!cookieService.Authenticated || (await cookieService.GetCurrentUser()).ProfileType != "tutor")
            {
                return Redirect(cookieService.Url("/Home/SignIn/SW"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, new string[] { CurrentUserProfile.Username });
            ChatInfo = UserChannel.ArchivedVideoId != null ? encryptionService.EncryptString(UserChannel.ArchivedVideoId) : encryptionService.EncryptString("DEFAULT");

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);

            ScheduledStream = await storageService.Get<Schedule>(SQLQueries.GetScheduleWithId, new string[] { scheduleId, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") });

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
            var userChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, userProfile.Username);

            var archivedVideoId = streamService.StartStream(Request, userProfile, userChannel);
            if (archivedVideoId != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = new string[] { encryptionService.EncryptString(archivedVideoId), userChannel.Username } }) ;
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
