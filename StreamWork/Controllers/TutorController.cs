using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using StreamWork.ViewModels;
using StreamWork.Threads;
using StreamWork.HelperClasses;
using System;
using System.Security.Claims;

namespace StreamWork.Controllers
{
    public class TutorController : Controller
    {
        readonly HomeMethods _homeMethods = new HomeMethods();
        readonly TutorMethods _tutorMethods = new TutorMethods();
        readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();
        readonly FollowingMethods _followingMethods = new FollowingMethods();
        readonly StreamClientMethods _streamClientMethods = new StreamClientMethods();

        [HttpGet]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-TutorStream");

            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name);

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                UserChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, User.Identity.Name),
                UserArchivedVideos = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name),
                ChatInfo = _encryptionMethods.EncryptString(userProfile.Username + "|" + userProfile.Id + "|" + userProfile.EmailAddress),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string change)
        {
            var user = HttpContext.User.Identity.Name;
            var userChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, user);
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
            var chatColor = "";

            foreach (var claims in HttpContext.User.Claims)
            {
                if (claims.Type == ClaimTypes.UserData) chatColor = claims.Value;
            }

            var startStream = _tutorMethods.StartStream(storageConfig, Request, userChannel, userProfile, chatColor);
            if(startStream) return Json(new { Message = JsonResponse.Success.ToString() });
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-ProfileTutor");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                UserChannels = await _homeMethods.GetUserChannels(storageConfig, SQLQueries.GetUserChannelWithUsername, User.Identity.Name),
                UserArchivedVideos = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name),
                NumberOfStreams = (await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name)).Count,
                Recommendations = await _homeMethods.GetRecommendationsForTutor(storageConfig, User.Identity.Name),
            };

            int viewCount = 0;
            foreach (var archivedStream in viewModel.UserArchivedVideos)
            {
                viewCount += archivedStream.Views;
            }

            viewModel.NumberOfViews = viewCount;
            viewModel.NumberOfFollowers = await _followingMethods.GetNumberOfFollowers(storageConfig, viewModel.TutorUserProfile.Id);
            viewModel.Schedule = _tutorMethods.GetTutorStreamSchedule(viewModel.UserChannels[0]);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> StreamCalendarUtil([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamName, DateTime dateTime, DateTime originalDateTime, bool remove) //StreamTask
        {
            var user = HttpContext.User.Identity.Name;
            var userChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, user);

            //Adds streams to schedule
            if (originalDateTime.Year == 1 && remove == false)
            {

                if (await _tutorMethods.AddStreamTask(storageConfig, streamName, dateTime, userChannel))
                    return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //Updates streams in schedule
            if (originalDateTime.Year != 1 && remove == false)
            {
                if (await _tutorMethods.UpdateStreamTask(storageConfig, streamName, dateTime, originalDateTime, userChannel))
                    return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //Removes streams in schedule
            if (originalDateTime.Year != 1 && remove)
            {
                if (await _tutorMethods.RemoveStreamTask(storageConfig, streamName, originalDateTime, userChannel))
                    return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> ClearRecommendation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await _tutorMethods.ClearRecommendation(storageConfig, id);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpPost]
        public IActionResult CheckIfStreamIsLive([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string channelKey)
        {
            bool response = _streamClientMethods.CheckIfUserChannelIsLive(channelKey);
            if (response == true) return Json(new { Message = JsonResponse.Success.ToString() });
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> TutorSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-TutorSettings");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                UserChannels = await _homeMethods.GetUserChannels(storageConfig, SQLQueries.GetUserChannelWithUsername, User.Identity.Name),
                UserArchivedVideos = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await _tutorMethods.DeleteStream(storageConfig, id);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveEditedStreamInfo([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (Request.Form.Files.Count != 0) //Edited info with new thumbnail
            {
                var streamInfo = Request.Form.Files[0].Name.Split(new char[] { '|' });

                var videoId = streamInfo[0];
                var streamTitle = streamInfo[1];
                var streamDescription = streamInfo[2];
                var streamThumbnail = _homeMethods.SaveIntoBlobContainer(Request.Form.Files[0], videoId, 1280, 720);

                var archivedVideo = await _homeMethods.GetArchivedStream(storageConfig, SQLQueries.GetArchivedStreamsWithId, videoId);
                archivedVideo.StreamTitle = streamTitle;
                archivedVideo.StreamDescription = streamDescription;
                archivedVideo.StreamThumbnail = streamThumbnail;

                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", archivedVideo.Id } }, archivedVideo);

                return Json(new { Message = JsonResponse.Success.ToString(), title = streamTitle, description = streamDescription, thumbnail = streamThumbnail });
            }

            if (Request.Form.Count != 0) //Edited info without new thumbnail
            {
                var streamInfo = new string[3];
                foreach (var key in Request.Form.Keys)
                    streamInfo = key.Split(new char[] { '|' });

                var videoId = streamInfo[0];
                var streamTitle = streamInfo[1];
                var streamDescription = streamInfo[2];

                var archivedVideo = await _homeMethods.GetArchivedStream(storageConfig, SQLQueries.GetArchivedStreamsWithId, videoId);
                archivedVideo.StreamTitle = streamTitle;
                archivedVideo.StreamDescription = streamDescription;

                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", archivedVideo.Id } }, archivedVideo);

                return Json(new { Message = JsonResponse.Success.ToString(), title = streamTitle, description = streamDescription, thumbnail = archivedVideo.StreamThumbnail });
            }
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        public async Task<IActionResult> TutorWatch([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-TutorStream");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),

                SearchViewModel = new SearchViewModel
                {
                    PopularStreamTutors = await _homeMethods.GetPopularStreamTutor(storageConfig),
                    StreamResults = await _homeMethods.SearchUserChannels(storageConfig, s, q),
                    ArchiveResults = await _homeMethods.SearchArchivedStreams(storageConfig, s, q),
                    UserProfile = User.Identity.Name == null ? null : await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                    Subject = string.IsNullOrEmpty(s) ? "All Subjects" : s,
                    SearchQuery = q,
                    SubjectIcon = _homeMethods.GetSubjectIcon(s)
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = HttpContext.User.Identity.Name;

            if (currentPassword != null && newPassword != null && confirmPassword != null)
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
                if (_encryptionMethods.DecryptPassword(userProfile.Password, currentPassword) == userProfile.Password)
                {
                    userProfile.Password = _encryptionMethods.EncryptPassword(newPassword);
                    await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                    return Json(new { Message = JsonResponse.Success.ToString() });
                }
                return Json(new { Message = JsonResponse.Failed.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
