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

namespace StreamWork.Controllers
{
    public class TutorController : Controller
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        readonly TutorHelperFunctions _tutorHelperFunctions = new TutorHelperFunctions();
        readonly EditProfileHelperFunctions _editProfileHelperFunctions = new EditProfileHelperFunctions();
        readonly ThreadClassHelperFunctions _threadClassHelperFunctions = new ThreadClassHelperFunctions();
       

        [HttpGet]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Tutor-TutorStream");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                UserChannel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, User.Identity.Name),
                UserArchivedVideos = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, User.Identity.Name),
            };

            viewModel.ChatSecretKey = await _homeHelperFunctions.GetChatSecretKey(storageConfig, viewModel.UserChannel.ChatId, User.Identity.Name);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string change)
        {
            var user = HttpContext.User.Identity.Name;
            var userChannel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, user);
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);

            //Saves streamTitle, URl, and subject into sql database
            if (Request.Form.Files.Count != 0)
            {
                var streamInfo = Request.Form.Files[0].Name.Split(new char[] { '|' });
                var streamTitle = streamInfo[0];
                var streamSubject = streamInfo[1];
                var streamDescription = streamInfo[2];
                var notifyStudents = streamInfo[3];
                var streamThumbnail =  _homeHelperFunctions.SaveIntoBlobContainer(Request.Form.Files[0], userChannel.Id, 1280, 720);

                ThreadClass handleStreams = new ThreadClass(storageConfig, userChannel, userProfile, streamTitle, streamSubject, streamDescription, streamThumbnail);
                handleStreams.RunLiveThread();
                if(notifyStudents.Equals("yes")) handleStreams.RunEmailThread();

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //Saves if there is no thumbnail uploaded
            if (Request.Form.Count != 0)
            {
                var streamInfo = new string[4];
                foreach (var key in Request.Form.Keys)
                    streamInfo = key.Split(new char[] { '|' });

                var streamTitle = streamInfo[0];
                var streamSubject = streamInfo[1];
                var streamDescription = streamInfo[2];
                var notifyStudents = streamInfo[3];

                ThreadClass handleStreams = new ThreadClass(storageConfig, userChannel, userProfile, streamTitle, streamSubject, streamDescription, _tutorHelperFunctions.GetCorrespondingDefaultThumbnail(streamSubject));
                handleStreams.RunLiveThread();
                if(notifyStudents.Equals("yes")) handleStreams.RunEmailThread();

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //change stream subject
            if (change != null)
            {
                userChannel.StreamSubject = change;
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Tutor-ProfileTutor");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                UserChannels = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, User.Identity.Name),
                UserArchivedVideos = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, User.Identity.Name),
                NumberOfStreams = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, User.Identity.Name)).Count,
                Recommendations = await _homeHelperFunctions.GetRecommendationsForTutor(storageConfig, User.Identity.Name),
            };

            viewModel.NumberOfFollowers = _tutorHelperFunctions.GetNumberOfFollowers(viewModel.TutorUserProfile);
            viewModel.Schedule = _tutorHelperFunctions.GetTutorStreamSchedule(viewModel.UserChannels[0]);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string remove)
        {
            var user = HttpContext.User.Identity.Name;
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);

            //Handles if there is a profile picture with the caption or about paragraph
            if (Request.Form.Files.Count > 0)
            {
                var success = await _editProfileHelperFunctions.EditProfileWithProfilePicture(Request, storageConfig, userProfile, user);
                if(success)
                    return Json(new { Message = JsonResponse.Success.ToString()});
            }

            //Handles if there is not a profile picture with the caption or about paragraph
            if (Request.Form.Keys.Count == 1)
            {
                var success = await _editProfileHelperFunctions.EditProfileWithNoProfilePicture(Request, storageConfig, user);
                if(success)
                    return Json(new { Message = JsonResponse.Success.ToString()});
            }

            return Json(new { Message = JsonResponse.Failed.ToString()});
        }


        [HttpPost]
        public async Task<IActionResult> StreamCalendarUtil([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamName, DateTime dateTime, DateTime originalDateTime, bool remove) //StreamTask
        {
            var user = HttpContext.User.Identity.Name;
            var userChannel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, user);

            //Adds streams to schedule
            if (originalDateTime.Year == 1 && remove == false)
            {
                
                if (await _tutorHelperFunctions.AddStreamTask(storageConfig, streamName, dateTime, userChannel))
                    return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //Updates streams in schedule
            if (originalDateTime.Year != 1 && remove == false)
            {
                if (await _tutorHelperFunctions.UpdateStreamTask(storageConfig, streamName, dateTime, originalDateTime, userChannel))
                    return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //Removes streams in schedule
            if (originalDateTime.Year != 1 && remove)
            {
                if (await _tutorHelperFunctions.RemoveStreamTask(storageConfig, streamName, originalDateTime, userChannel))
                    return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> ClearRecommendation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await _tutorHelperFunctions.ClearRecommendation(storageConfig, id);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpPost]
        public IActionResult CheckIfStreamIsLive([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string channelKey)
        {
            bool response = _threadClassHelperFunctions.CheckIfUserChannelIsLive(channelKey);
            if (response == true) return Json(new { Message = JsonResponse.Success.ToString()});
            return Json(new { Message = JsonResponse.Failed.ToString()});
        }

        [HttpGet]
        public async Task<IActionResult> TutorSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Tutor-TutorSettings");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                UserChannels = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, User.Identity.Name),
                UserArchivedVideos = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, User.Identity.Name)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await _tutorHelperFunctions.DeleteStream(storageConfig, id);
            return Json(new { Message = JsonResponse.Success.ToString()});
        }

        public async Task<IActionResult> TutorWatch([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Tutor-TutorStream");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),

                SearchViewModel = new SearchViewModel
                {
                    PopularStreamTutors = await _homeHelperFunctions.GetPopularStreamTutor(storageConfig),
                    StreamResults = await _homeHelperFunctions.SearchUserChannels(storageConfig, s, q),
                    ArchiveResults = await _homeHelperFunctions.SearchArchivedStreams(storageConfig, s, q),
                    UserProfile = User.Identity.Name == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                    Subject = string.IsNullOrEmpty(s) ? "All Subjects" : s,
                    SearchQuery = q,
                    SubjectIcon = _homeHelperFunctions.GetSubjectIcon(s)
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
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);
                if (_homeHelperFunctions.DecryptPassword(userProfile.Password, currentPassword) == userProfile.Password)
                {
                    userProfile.Password = _homeHelperFunctions.EncryptPassword(newPassword);
                    await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                    return Json(new { Message = JsonResponse.Success.ToString() });
                }
                return Json(new { Message = JsonResponse.Failed.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
