using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using StreamWork.ViewModels;
using StreamWork.DataModels;
using StreamWork.HelperClasses;
using Microsoft.Extensions.Primitives;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        readonly TutorHelperFunctions _tutorHelperFunctions = new TutorHelperFunctions();
        readonly FollowingHelperFunctions _followingHelperFunctions = new FollowingHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {

            var populatePage = await _homeHelperFunctions.PopulateHomePage(storageConfig, HttpContext.User.Identity.Name);
            populatePage.IsUserFollowingThisTutor = false;

            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);
                populatePage.UserProfile = userProfile;

                if (populatePage.UserProfile.FollowedStudentsAndTutors != null && populatePage.UserChannel != null)
                    populatePage.IsUserFollowingThisTutor = populatePage.UserProfile.FollowedStudentsAndTutors.Contains(populatePage.UserChannel.Id);

                return View(populatePage);
            }

            return View(populatePage);
        }

        public async Task<IActionResult> Subject([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s)
        { //s is subject
            return View(await _homeHelperFunctions.PopulateSubjectPage(storageConfig, s, HttpContext.User.Identity.Name));
        }

        public async Task<IActionResult> Search([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q) {
            // s is subject, q is search query
            return View(await _homeHelperFunctions.PopulateSearchPage(storageConfig, s, q, HttpContext.User.Identity.Name));
        }

        public async Task<IActionResult> BecomeTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else
            {
                viewModel = new DefaultViewModel { };
            }
            return View(viewModel);
        }

        public async Task<IActionResult> About([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else
            {
                viewModel = new DefaultViewModel { };
            }
            return View(viewModel);
        }

        public async Task<IActionResult> HowToStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else
            {
                viewModel = new DefaultViewModel { };
            }
            return View(viewModel);
        }

        public async Task<IActionResult> PickStudentOrTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else
            {
                viewModel = new DefaultViewModel { };
            }
            return View(viewModel);
        }

        public IActionResult SplashPage()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string tutor)
        {
            ProfileTutorViewModel profile = new ProfileTutorViewModel
            {
                UserChannels = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, tutor),
                UserArchivedVideos = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, tutor),
                UserProfile = null,
                StudentOrTutorProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                NumberOfStreams = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, tutor)).Count
            };

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                profile.UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name);

                if (profile.UserProfile.FollowedStudentsAndTutors != null)
                    profile.IsUserFollowingThisTutor = profile.UserProfile.FollowedStudentsAndTutors.Contains(profile.UserChannels[0].Id);
            }
            else
            {
                profile.IsUserFollowingThisTutor = false;
            }

            profile.Schedule = _tutorHelperFunctions.GetTutorStreamSchedule(profile.UserChannels[0]);

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileView([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string tutorId, string followRequest, string unFollowRequest)
        {
            //handles follow requests
            if (followRequest != null && tutorId != null)
            {
                var user = HttpContext.User.Identity.Name;
                var userLogin = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user);

                var tutorChannel = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannelFromId, tutorId);
                var tutorLogin = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, tutorChannel[0].Username);

                var updatedUserList = _followingHelperFunctions.AddToListOfFollowedTutors(tutorId, userLogin[0].FollowedStudentsAndTutors);
                var updatedTutorList = _followingHelperFunctions.AddToListOfFollowedStudents(userLogin[0].EmailAddress, tutorLogin[0].FollowedStudentsAndTutors);

                userLogin[0].FollowedStudentsAndTutors = updatedUserList;
                tutorLogin[0].FollowedStudentsAndTutors = updatedTutorList;

                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userLogin[0].Id } }, userLogin[0]);
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", tutorLogin[0].Id } }, tutorLogin[0]);

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            //handles unfollow requests
            if (unFollowRequest != null && tutorId != null)
            {
                var user = HttpContext.User.Identity.Name;
                var userLogin = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user);

                var tutorChannel = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannelFromId, tutorId);
                var tutorLogin = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, tutorChannel[0].Username);

                var updatedUserList = _followingHelperFunctions.RemoveFromListOfFollowedTutors(tutorId, userLogin[0].FollowedStudentsAndTutors);
                var updatedTutorList = _followingHelperFunctions.RemoveFromListOfFollowedStudents(userLogin[0].EmailAddress, tutorLogin[0].FollowedStudentsAndTutors);

                userLogin[0].FollowedStudentsAndTutors = updatedUserList;
                tutorLogin[0].FollowedStudentsAndTutors = updatedTutorList;

                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userLogin[0].Id } }, userLogin[0]);
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", tutorLogin[0].Id } }, tutorLogin[0]);

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecommendation ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string student, string tutor, string recommendation) {
            await _homeHelperFunctions.SaveRecommendation(storageConfig, student, tutor, recommendation);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string payPalAddress, string username, string password, string passwordConfirm, string college, string role)
        {

            if (password == null)
            {
                var checkCurrentUsers = await DataStore.GetListAsync<UserLogin>(_homeHelperFunctions._connectionString, storageConfig.Value, "CurrentUser", new List<string> { username });
                if (checkCurrentUsers.Count == 0)
                {
                    return Json(new { Message = JsonResponse.Success.ToString() });
                }
                else
                {
                    return Json(new { Message = JsonResponse.Failed.ToString() });
                }
            }

            UserLogin signUpProfile = new UserLogin
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameFirst + "|" + nameLast,
                EmailAddress = email,
                Username = username,
                Password = _homeHelperFunctions.EncryptPassword(password),
                ProfileType = role,
                AcceptedTutor = false,
                College = college,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                Balance = (decimal)0f,
                Expiration = DateTime.UtcNow,
                TrialAccepted = false,
                PayPalAddress = payPalAddress
            };
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProfile.Id } }, signUpProfile);

            if (role == "tutor")
            {
                //Create User Channel For Tutor
                UserChannel userChannel = new UserChannel
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = username,
                    ChannelKey = null,
                    StreamSubject = null,
                    StreamThumbnail = null,
                    StreamTitle = null,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                    ChatId = _homeHelperFunctions.FormatChatId(DataStore.GetChatID("https://www.cbox.ws/apis/threads.php?id=6-829647-oq4rEn&key=ae1682707f17dbc2c473d946d2d1d7c3&act=mkthread"))
                };
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);

                //Checks for the attachments that tutors provide and sends them to streamwork for verification
                if (Request.Form.Files.Count != 0)
                {
                    List<string> values = new List<string>();
                    foreach (var key in Request.Form.Keys)
                    {
                        Request.Form.TryGetValue(key, out StringValues value);
                        values.Add(value);
                    }

                    var files = Request.Form.Files;
                    List<Attachment> attachments = new List<Attachment>();
                    foreach (var file in files)
                        attachments.Add(new Attachment(file.OpenReadStream(), file.FileName));
                    await _homeHelperFunctions.SendEmailToAnyEmailAsync(_homeHelperFunctions._streamworkEmailID, _homeHelperFunctions._streamworkEmailID, "Tutor Evaluation", email, attachments);
                }
            }

            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string password)
        {
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if (userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString() });

            var checkforUser = await DataStore.GetListAsync<UserLogin>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.AllSignedUpUsersWithPassword.ToString(), new List<string> { username, _homeHelperFunctions.DecryptPassword(userProfile.Password, password) });
            if (checkforUser.Count == 1)
            {
                HttpContext.Session.SetString(QueryHeaders.UserProfile.ToString(), username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, userProfile.EmailAddress),
                };

                var userIdentity = new ClaimsIdentity(claims, "cookie");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (userProfile.ProfileType == "tutor")
                    return Json(new { Message = JsonResponse.Tutor.ToString() });
                else
                    return Json(new { Message = JsonResponse.Student.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username)
        {
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if (userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString() });

            Random random = new Random();
            string key = Convert.ToString(random.Next(int.MaxValue), 16);
            userProfile.ChangePasswordKey = key;
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            await _homeHelperFunctions.SendEmailToAnyEmailAsync(_homeHelperFunctions._streamworkEmailID, userProfile.EmailAddress, "Password Recovery", _homeHelperFunctions.CreateUri(userProfile.Username, key), null);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string newPassword, string confirmNewPassword, string username, string key) {
            if (newPassword == confirmNewPassword) {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);

                if (!key.Equals(userProfile.ChangePasswordKey))
                    return Json(new { Message = JsonResponse.QueryFailed.ToString() });

                userProfile.Password = _homeHelperFunctions.EncryptPassword(newPassword);
                userProfile.ChangePasswordKey = null;
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string logout)
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Home-Subscribe");

            var user = HttpContext.User.Identity.Name;

            DefaultViewModel model = new DefaultViewModel {
                UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Donate(string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Home-Donate?tutor=" + tutor);

            var user = HttpContext.User.Identity.Name;

            ProfileTutorViewModel model = new ProfileTutorViewModel
            {
                UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                StudentOrTutorProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
            };

            return View(model);
        }
    }
}
