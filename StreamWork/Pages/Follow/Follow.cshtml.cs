using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.Services;

namespace StreamWork.Pages.Follow
{
    public class Follow : PageModel //not sure if this the right way to do it but we are going to have follow buttons a lot over the site and I didnt want to make a onPost method in each of those cshtmls + this makes the ajax call easier
    {
        private readonly FollowService followService;

        public Follow(FollowService follow)
        {
            followService = follow;
        }

        public async Task OnPostFollow(string followerId, string followeeId)
        {
            await followService.AddFollower(followerId, followeeId);
        }

        public async Task OnPostUnfollow(string followerId, string followeeId)
        {
            await followService.AddFollower(followerId, followeeId);
        }
    }
}
