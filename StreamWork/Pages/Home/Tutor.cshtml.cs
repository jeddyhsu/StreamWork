using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class TutorModal : PageModel
    {
        private readonly CookieService cookieService;
        private readonly NotificationService notificationService;

        public DataModels.Profiles CurrentUserProfile { get; set; }
        public List<string> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public TutorModal(CookieService cookie, NotificationService notification)
        {
            cookieService = cookie;
            notificationService = notification;
        }

        public async Task<IActionResult> OnGet()
        {
            CurrentUserProfile = await cookieService.GetCurrentUser();

            if (CurrentUserProfile != null)
            {
                Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
                AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            }

            return Page();
        }
    }
}
