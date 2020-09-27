using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Notifications
{
    public class NotificationModel : PageModel
    {
        private readonly NotificationService notificationService;

        public NotificationModel(NotificationService notification)
        {
            notificationService = notification;
        }

        public async Task<IActionResult> OnPostUpdateToSeen(string username)
        {
            var savedInfo = await notificationService.UpdateNotificationsToSeen(username);

            if (savedInfo) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteNotification(string id)
        {
            var savedInfo = await notificationService.DeleteNotification(id);

            if (savedInfo) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
