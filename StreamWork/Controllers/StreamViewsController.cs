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
            var split = streamKeyandchatId.Split(new char[] { '|' }); //spilt[0] = video channel key, split[1] = channel chat id, split[2] = chat key, split[3] = channel username, split[4] = stream title
            var secretChatKey = _homehelperFunctions.GetChatSecretKey(split[1], split[2], User.Identity.Name);
            var channel = await _homehelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, split[3]);
            string[] arr = { split[0], secretChatKey, split[4], channel[0].Id };

            ProfileTutorViewModel profile = new ProfileTutorViewModel {
                UserProfile = User.Identity.Name != null ? await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name) : null,
                StudentOrTutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, split[3]),
                NumberOfStreams = (await _homehelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, channel[0].Username)).Count
            };

            if (profile.UserProfile != null && profile.UserProfile.FollowedTutors != null)
                profile.IsUserFollowingThisTutor = profile.UserProfile.FollowedTutors.Contains(channel[0].Id);

            StreamPageViewModel model = new StreamPageViewModel {
                UserProfile = profile.UserProfile,
                Profile = profile,
                UrlParams = arr
            };

            return View ("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPlaybackPage?streamId=" + streamId);

            var archivedStreams = await _homehelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.ArchivedVideosByStreamId, streamId);
            var channel = await _homehelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, archivedStreams[0].Username);

            string[] arr = { streamId, archivedStreams[0].StreamTitle,channel[0].Id};

            ProfileTutorViewModel profile = new ProfileTutorViewModel {
                UserProfile = User.Identity.Name != null ? await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name) : null,
                StudentOrTutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, channel[0].Username),
                NumberOfStreams = (await _homehelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, channel[0].Username)).Count
            };

            if (profile.UserProfile.FollowedTutors != null)
                profile.IsUserFollowingThisTutor = profile.UserProfile.FollowedTutors.Contains(channel[0].Id);

            StreamPageViewModel model = new StreamPageViewModel {
                UserProfile = profile.UserProfile,
                Profile = profile,
                UrlParams = arr
            };

            return View("StreamPlaybackPage", model);
        }
    }
}