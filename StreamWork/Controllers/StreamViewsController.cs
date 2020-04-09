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
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamTutorUsername)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPage?streamTutorUsername=" + streamTutorUsername);

            var channel = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, streamTutorUsername);
            var chatBox = await _homeHelperFunctions.GetChatSecretKey(storageConfig, channel[0].ChatId, HttpContext.User.Identity.Name);

            if(channel[0].StreamTitle == null)
                return Redirect("https://www.streamwork.live/Home/ProfileView?Tutor=" + channel[0].Username);

            StreamPageViewModel model = new StreamPageViewModel
            {
                UserProfile = HttpContext.User.Identity.Name != null ? (await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name))[0] : null,
                ChatBox = chatBox,
                UserChannel = channel[0]
            };

            if (model.UserProfile != null && model.UserProfile.FollowedStudentsAndTutors != null)
                model.IsUserFollowingThisTutor = model.UserProfile.FollowedStudentsAndTutors.Contains(channel[0].Id);

            return View ("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPlaybackPage?streamId=" + streamId);

            var archivedStream = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.ArchivedVideosByStreamId, streamId);
            var channel = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, archivedStream[0].Username);

            StreamPlayBackPageViewModel model = new StreamPlayBackPageViewModel
            {
                UserProfile = (await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name))[0],
                TutorUserProfile = (await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, channel[0].Username))[0],
                ArchivedStream = archivedStream[0],
                UserChannel = channel[0],
                ArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos),
                NumberOfStreams = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, channel[0].Username)).Count
            };

            if (model.UserProfile != null && model.UserProfile.FollowedStudentsAndTutors != null)
                model.IsUserFollowingThisTutor = model.UserProfile.FollowedStudentsAndTutors.Contains(channel[0].Id);

            return View("StreamPlaybackPage", model) ;
        }
    }
}