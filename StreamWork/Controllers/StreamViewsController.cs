using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.ViewModels;
using StreamWork.HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StreamWork.Config;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        HomeHelperFunctions _homehelperFunctions = new HomeHelperFunctions();

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamKeyandchatId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPage");

            var split = streamKeyandchatId.Split(new char[] { '|' });
            var secretChatKey = _homehelperFunctions.GetChatSecretKey(split[1], split[2], User.Identity.Name);
            string[] arr = { split[0], secretChatKey, split[4] };

            ProfileTutorViewModel profile = new ProfileTutorViewModel {
                userProfile = User.Identity.Name != null ? await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name) : null,
                studentOrtutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, split[3])
            };

            if (profile.userProfile.FollowedTutors != null)
                profile.isUserFollowingThisTutor = profile.userProfile.FollowedTutors.Contains(profile.userChannels[0].Id);

            StreamPageViewModel model = new StreamPageViewModel {
                userProfile = profile.userProfile,
                profile = profile,
                urlParams = arr
            };

            return View ("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPlaybackPage");

            var archivedStreams = await _homehelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.ArchivedVideosByStreamId, streamId);

            string[] arr = { streamId, archivedStreams[0].StreamTitle };

            ProfileTutorViewModel profile = new ProfileTutorViewModel {
                userProfile = User.Identity.Name != null ? await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name) : null,
                studentOrtutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name)
            };

            if (profile.userProfile.FollowedTutors != null)
                profile.isUserFollowingThisTutor = profile.userProfile.FollowedTutors.Contains(profile.userChannels[0].Id);

            StreamPageViewModel model = new StreamPageViewModel {
                userProfile = profile.userProfile,
                profile = profile,
                urlParams = arr
            };

            return View("StreamPlaybackPage", model);
        }
    }
}