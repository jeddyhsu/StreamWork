using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using StreamWork.ViewModels;
using StreamWork.ViewModels.Tutor;
using StreamWork.Threads;
using StreamWork.HelperMethods;
using System;
using System.Security.Claims;

namespace StreamWork.Controllers
{
    public class TutorController : Controller
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly TutorMethods _tutorMethods = new TutorMethods();
        private readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();
        private readonly FollowingMethods _followingMethods = new FollowingMethods();
        private readonly StreamClientMethods _streamClientMethods = new StreamClientMethods();
        private readonly EditProfileMethods _editProfileMethods = new EditProfileMethods();
        private readonly ScheduleMethods _scheduleMethods = new ScheduleMethods();

        [HttpGet]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-TutorStream");

            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name);

            TutorStreamViewModel viewModel = new TutorStreamViewModel
            {
                UserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                UserChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, User.Identity.Name),
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
        public async Task<IActionResult> TutorDashboard([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-TutorDashboard");

            var user = HttpContext.User.Identity.Name;
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);

            TutorDashboardViewModel viewModel = new TutorDashboardViewModel
            {
                UserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                UserChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, User.Identity.Name),
                UserArchivedStreams = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name),
                NumberOfStreams = (await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name)).Count,
                Sections = _tutorMethods.GetSections(userProfile),
                Topics = _tutorMethods.GetTopics(userProfile),
                Recommendations = await _homeMethods.GetRecommendationsForTutor(storageConfig, User.Identity.Name),
            };

            int viewCount = 0;
            foreach (var archivedStream in viewModel.UserArchivedStreams)
            {
                viewCount += archivedStream.Views;
            }

            viewModel.NumberOfViews = viewCount;
            viewModel.NumberOfFollowers = await _followingMethods.GetNumberOfFollowers(storageConfig, viewModel.UserProfile.Id);
            viewModel.Schedule = await _scheduleMethods.GetSchedule(storageConfig, user);

            return View(viewModel);
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

            TutorDashboardViewModel viewModel = new TutorDashboardViewModel
            {
                UserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
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
            var saved = await _tutorMethods.EditArchivedStream(storageConfig, Request);
            return Json(new { Message = JsonResponse.Success.ToString(), title = saved[0], description = saved[1], thumbnail = saved[2] });
        }

        public async Task<IActionResult> TutorWatch([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Tutor-TutorStream");

            TutorWatchViewModel viewModel = new TutorWatchViewModel
            {
                UserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),

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


        public async Task<IActionResult> HowToStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    GenericUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, HttpContext.User.Identity.Name)
                };
            }
            else viewModel = new DefaultViewModel { };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.User.Identity.Name;
            
            var saved = await _editProfileMethods.EditProfile(storageConfig, Request, user);
            if(saved != null) return Json(new { Message = JsonResponse.Success.ToString(), firstName = saved[0], lastName = saved[1], occupation = saved[2], location = saved[3], timezone = saved[4], linkedInUrl = saved[5], profilePicture = saved[6] });
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSection([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.User.Identity.Name;
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);

            if (_tutorMethods.SaveSection(Request, userProfile))
            {
                return Json(new { Message = JsonResponse.Success.ToString() });
            }
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveTopic([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.User.Identity.Name;
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);

            if (_tutorMethods.SaveTopic(Request, userProfile))
            {
                return Json(new { Message = JsonResponse.Success.ToString() });
            }
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveBanner([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.User.Identity.Name;
            var banner = await _editProfileMethods.SaveBanner(storageConfig, Request, user);

            if (banner != null)
            {
                return Json(new { Message = JsonResponse.Success.ToString(), Banner = banner });
            }
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveUniversity([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string abbr, string name)
        {
            var user = HttpContext.User.Identity.Name;

            if (await _editProfileMethods.SaveUniversity(storageConfig, user, abbr, name))
            {
                return Json(new { Message = JsonResponse.Success.ToString() });
            }
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveScheduleTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.User.Identity.Name;
            var sortedList = await _scheduleMethods.SaveToSchedule(storageConfig, Request, user);
            if (sortedList != null)
            {
                return Json(new { Message = JsonResponse.Success.ToString(), Sorted = sortedList });
            }
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteScheduleTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string taskId)
        {
            var user = HttpContext.User.Identity.Name;
            var sortedList = await _scheduleMethods.DeleteFromSchedule(storageConfig, taskId, user);
            if (sortedList != null)
            {
                return Json(new { Message = JsonResponse.Success.ToString(), Sorted = sortedList });
            }
            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> SearchArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string searchTerm, string filter)
        {
            return Json(new { Message = JsonResponse.Success.ToString(), Results = await _homeMethods.SearchArchivedStreams(storageConfig, filter, searchTerm) });
        }
    }
}
