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
        readonly StreamHelperFunctions _streamHelperFunctions = new StreamHelperFunctions();

        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamTutorUsername)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPage?streamTutorUsername=" + streamTutorUsername);

            var channel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, streamTutorUsername);
            var tutorProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, streamTutorUsername);
            var chatBox = await _homeHelperFunctions.GetChatSecretKey(storageConfig, channel.ChatId, HttpContext.User.Identity.Name);

            //if(channel.StreamTitle == null)
            //    return Redirect("https://www.streamwork.live/Home/ProfileView?Tutor=" + channel.Username);\

            var student = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);

            StreamPageViewModel model = new StreamPageViewModel();
            if(student.ProfileType != "tutor") model.StudentUserProfile = student;
            else model.TutorUserProfile = student;
            model.TutorStreamingUserProfile = tutorProfile;
            model.ChatBox = chatBox;
            model.UserChannel = channel;

            if (student != null && student.FollowedStudentsAndTutors != null)
                model.IsUserFollowingThisTutor = student.FollowedStudentsAndTutors.Contains(tutorProfile.Id);

            await _streamHelperFunctions.IncrementChannelViews(storageConfig, HttpContext.User.Identity.Name, streamTutorUsername);

            return View ("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPlaybackPage?streamId=" + streamId);

            var archivedStream = await _homeHelperFunctions.GetArchivedStream(storageConfig, QueryHeaders.ArchivedVideosByStreamId, streamId);
            var channel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, archivedStream.Username);
            var tutorProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, archivedStream.Username);

            StreamPlayBackPageViewModel model = new StreamPlayBackPageViewModel
            {
                StudentUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name), //just a viwer
                TutorUserProfile = tutorProfile,
                ArchivedStream = archivedStream,
                UserChannel = channel,
                ArchivedStreams = await _homeHelperFunctions.GetAllArchivedStreams(storageConfig),
                NumberOfStreams = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, channel.Username)).Count
            };

            if (model.StudentUserProfile != null && model.StudentUserProfile.FollowedStudentsAndTutors != null)
                model.IsUserFollowingThisTutor = model.StudentUserProfile.FollowedStudentsAndTutors.Contains(tutorProfile.Id);

            await _streamHelperFunctions.IncrementArchivedVideoViews(storageConfig, HttpContext.User.Identity.Name, streamId);

            return View("StreamPlaybackPage", model) ;
        }
    }
}