using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.ViewModels;
using StreamWork.HelperMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StreamWork.Config;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        readonly HomeMethods _homeMethods = new HomeMethods();
        readonly StreamMethods _streamMethods = new StreamMethods();
        readonly FollowingMethods _followingMethods = new FollowingMethods();
        readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();

        [HttpGet]
        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamTutorUsername, string id) //id is archivedVideo id
        {
            if (HttpContext.User.Identity.IsAuthenticated == false && id == null) return Redirect(_homeMethods._host + "/Home/Login?dest=-StreamViews-StreamPage?streamTutorUsername=" + streamTutorUsername);
            else if (HttpContext.User.Identity.IsAuthenticated == false && id != null) return Redirect(_homeMethods._host + "/Home/Login?dest=-StreamViews-StreamPage?streamTutorUsername=" + streamTutorUsername + "&id=" + id);

            var channel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, streamTutorUsername);
            var tutorProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, streamTutorUsername);

            if (channel.StreamTitle == null && id != null)
            {
                id = id.Replace('/', '-');
                var archivedStream = await _homeMethods.GetArchivedStream(storageConfig, SQLQueries.GetArchivedStreamsWithId, id);
                if (archivedStream == null) return Redirect(_homeMethods._host + "/Home/ProfileView?Tutor=" + channel.Username);
                else return Redirect(_homeMethods._host + "/StreamViews/StreamPlaybackPage?streamId=" + archivedStream.StreamID);
            }

            var student = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, HttpContext.User.Identity.Name);

            StreamPageViewModel model = new StreamPageViewModel();
            if (student.ProfileType != "tutor") model.StudentUserProfile = student;
            else model.TutorUserProfile = student;
            model.TutorStreamingUserProfile = tutorProfile;
            model.UserChannel = channel;
            model.ChatInfo = _encryptionMethods.EncryptString(student.Username + "|" + student.Id + "|" + student.EmailAddress);
            model.StreamSubjectPicture = _streamMethods.GetCorrespondingSubjectThumbnail(model.UserChannel.StreamSubject);

            if (student != null)
                model.IsFollowing = await _followingMethods.IsFollowingFollowee(storageConfig, student.Id, tutorProfile.Id);

            await _streamMethods.IncrementChannelViews(storageConfig, HttpContext.User.Identity.Name, streamTutorUsername);

            return View("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-StreamViews-StreamPlaybackPage?streamId=" + streamId);

            var archivedStream = await _homeMethods.GetArchivedStream(storageConfig, SQLQueries.GetArchivedStreamsWithStreamId, streamId);
            var channel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, archivedStream.Username);
            var tutorProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, archivedStream.Username);

            StreamPlayBackPageViewModel model = new StreamPlayBackPageViewModel
            {
                GenericUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, HttpContext.User.Identity.Name), //just a viewer
                TutorUserProfile = tutorProfile,
                ArchivedStream = archivedStream,
                UserChannel = channel,
                ArchivedStreams = await _homeMethods.GetAllArchivedStreams(storageConfig),
                NumberOfStreams = (await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, channel.Username)).Count
            };

            if (model.GenericUserProfile != null)
                model.IsFollowing = await _followingMethods.IsFollowingFollowee(storageConfig, model.GenericUserProfile.Id, model.TutorUserProfile.Id);

            await _streamMethods.IncrementArchivedVideoViews(storageConfig, HttpContext.User.Identity.Name, streamId);

            return View("StreamPlaybackPage", model);
        }
    }
}