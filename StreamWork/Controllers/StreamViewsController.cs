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

        [HttpGet]
        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamTutorUsername, string id) //id is archivedVideo id
        {
            if (HttpContext.User.Identity.IsAuthenticated == false && id == null)
            {
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPage?streamTutorUsername=" + streamTutorUsername);
            }
            else if (HttpContext.User.Identity.IsAuthenticated == false && id != null)
            {
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPage?streamTutorUsername=" + streamTutorUsername + "&id=" + id);
            }
           
            var channel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, streamTutorUsername);
            var tutorProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, streamTutorUsername);
           
            if(channel.StreamTitle == null && id != null)
            {
                id = id.Replace('/', '-');
                var archivedStream = await _homeHelperFunctions.GetArchivedStream(storageConfig, QueryHeaders.ArchivedVideosById, id);
                if(archivedStream == null) return Redirect(_homeHelperFunctions._host + "/Home/ProfileView?Tutor=" + channel.Username);
                else return Redirect(_homeHelperFunctions._host + "/StreamViews/StreamPlaybackPage?streamId=" + archivedStream.StreamID);
            }

            var student = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);

            StreamPageViewModel model = new StreamPageViewModel();
            if(student.ProfileType != "tutor") model.StudentUserProfile = student;
            else model.TutorUserProfile = student;
            model.TutorStreamingUserProfile = tutorProfile;
            model.UserChannel = channel;
            model.ChatInfo = _homeHelperFunctions.EncryptString(HttpContext.User.Identity.Name);
            model.StreamSubjectPicture = _streamHelperFunctions.GetCorrespondingSubjectThumbnail(model.UserChannel.StreamSubject);

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
                GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name), //just a viewer
                TutorUserProfile = tutorProfile,
                ArchivedStream = archivedStream,
                UserChannel = channel,
                ArchivedStreams = await _homeHelperFunctions.GetAllArchivedStreams(storageConfig),
                NumberOfStreams = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, channel.Username)).Count
            };

            if (model.GenericUserProfile != null && model.GenericUserProfile.FollowedStudentsAndTutors != null)
                model.IsUserFollowingThisTutor = model.GenericUserProfile.FollowedStudentsAndTutors.Contains(tutorProfile.Id);

            await _streamHelperFunctions.IncrementArchivedVideoViews(storageConfig, HttpContext.User.Identity.Name, streamId);

            return View("StreamPlaybackPage", model) ;
        }
    }
}