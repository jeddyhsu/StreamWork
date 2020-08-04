using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Follows
{
    public class FollowModel : PageModel //not sure if this the right way to do it but we are going to have follow buttons a lot over the site and I didnt want to make a onPost method in each of those cshtmls + this makes the ajax call easier
    {
        private readonly FollowService followService;
        private readonly NotificationService notificationService;

        public FollowModel(FollowService follow, NotificationService notification)
        {
            followService = follow;
            notificationService = notification;
        }

        public async Task OnPostFollow(string followerId, string followeeId)
        {
            var savedInfo = await followService.AddFollower(followerId, followeeId);
            await notificationService.SaveNotification(NotificationType.Follow, savedInfo[0], savedInfo[1], "", savedInfo[2]);
        }

        public async Task OnPostUnfollow(string followerId, string followeeId)
        {
            await followService.RemoveFollower(followerId, followeeId);
        }
    }
}
