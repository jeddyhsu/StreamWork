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
        readonly EmailHelperFunctions _emailHelperFunctions = new EmailHelperFunctions();
        readonly EditProfileHelperFunctions _editProfileHelperFunctions = new EditProfileHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var populatePage = await _homeHelperFunctions.PopulateHomePage(storageConfig, HttpContext.User.Identity.Name);
            populatePage.IsUserFollowingThisTutor = false;
            
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);
                populatePage.GenericUserProfile = userProfile;
                populatePage.ChatInfo = _homeHelperFunctions.EncryptString(userProfile.Username + "|" + userProfile.Id + "|" + userProfile.EmailAddress);

                if (populatePage.GenericUserProfile.FollowedStudentsAndTutors != null && populatePage.UserChannel != null)
                    populatePage.IsUserFollowingThisTutor = populatePage.GenericUserProfile.FollowedStudentsAndTutors.Contains(populatePage.UserChannel.Id);

                return View(populatePage);
            }

            return View(populatePage);
        }

        public async Task<IActionResult> Search([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q) {
            // s is subject, q is search query
            string user = HttpContext.User.Identity.Name;
            return View(new SearchViewModel
            {
                PopularStreamTutors = await _homeHelperFunctions.GetPopularStreamTutor(storageConfig),
                StreamResults = await _homeHelperFunctions.SearchUserChannels(storageConfig, s, q),
                ArchiveResults = await _homeHelperFunctions.SearchArchivedStreams(storageConfig, s, q),
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                Subject = string.IsNullOrEmpty(s) ? "All Subjects" : s,
                SearchQuery = q,
                SubjectIcon = _homeHelperFunctions.GetSubjectIcon(s)
            });
        }

        public async Task<IActionResult> BecomeTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            PopularStreamTutorsViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new PopularStreamTutorsViewModel
                {
                    GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else viewModel = new PopularStreamTutorsViewModel { };

            viewModel.PopularStreamTutors = await _homeHelperFunctions.GetPopularStreamTutor(storageConfig);

            return View(viewModel);
        }

        public async Task<IActionResult> About([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else viewModel = new DefaultViewModel { };

            return View(viewModel);
        }

        public async Task<IActionResult> HowToStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else viewModel = new DefaultViewModel { };

            return View(viewModel);
        }

        public async Task<IActionResult> PickStudentOrTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            DefaultViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new DefaultViewModel
                {
                    GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name)
                };
            }
            else viewModel = new DefaultViewModel { };

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
                GenericUserProfile = HttpContext.User.Identity.Name != null ? await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name) : null,
                TutorUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                UserChannel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, tutor),
                UserArchivedVideos = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, tutor),
                NumberOfStreams = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, tutor)).Count
            };

            profile.NumberOfFollowers = await _followingHelperFunctions.GetNumberOfFollowers(storageConfig,profile.TutorUserProfile.Id);
            if (HttpContext.User.Identity.IsAuthenticated) profile.IsFollowing = await _followingHelperFunctions.IsFollowingFollowee(storageConfig, profile.GenericUserProfile.Id, profile.TutorUserProfile.Id);
            profile.Schedule = _tutorHelperFunctions.GetTutorStreamSchedule(profile.UserChannel);

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> AddFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if(followerId != null && followeeId != null)
            {
                if(await _followingHelperFunctions.AddFollower(storageConfig, followerId, followeeId)) return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if (await _followingHelperFunctions.RemoveFollower(storageConfig, followerId, followeeId)) return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecommendation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string student, string tutor, string recommendation) {
            await _homeHelperFunctions.SaveRecommendation(storageConfig, student, tutor, recommendation);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                                                               string nameFirst,
                                                                                               string nameLast,
                                                                                               string email,
                                                                                               string payPalAddress,
                                                                                               string username,
                                                                                               string password,
                                                                                               string college,
                                                                                               string role)
        {

            if (password == null && email != null) //initial checks!
            {
                var checkUsername = await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.CurrentUser, username);
                if (checkUsername.Count != 0) return Json(new { Message = JsonResponse.UsernameExists.ToString() });

                var checkEmail = await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.CurrentUserUsingEmail, email);
                if (checkEmail.Count != 0) return Json(new { Message = JsonResponse.EmailExists.ToString() });

                if (payPalAddress != null) //only for tutors
                {
                    var checkPayPalEmailUsingRegularEmail = await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.CurrentUserUsingEmail, payPalAddress); //payPal email can't be someone elses regular email
                    var checkPayPalEmail = await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.CheckUserUsingPayPalAddress, payPalAddress);
                    if (checkPayPalEmailUsingRegularEmail.Count != 0 || checkPayPalEmail.Count != 0) return Json(new { Message = JsonResponse.PayPalEmailExists.ToString() });
                }

                return Json(new { Message = JsonResponse.Success.ToString()});
            }

            UserLogin userProfile = new UserLogin
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
                PayPalAddress = payPalAddress,
                NotificationSubscribe = DatabaseValues.True.ToString()
            };

            if (userProfile.ProfileType == "student") await _emailHelperFunctions.SendOutEmailToStreamWorkTeam(userProfile);
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            if (role == "tutor")
            {
                //Create User Channel For Tutor
                UserChannel userChannel = new UserChannel
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = username,
                    ChannelKey = _homeHelperFunctions._defaultStreamHosterChannelKey,
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
                    await _emailHelperFunctions.SendOutEmailToStreamWorkTeam(nameFirst, nameLast, email, attachments);
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
                    new Claim(ClaimTypes.UserData, _homeHelperFunctions.GetRandomChatColor())
                };

                var userIdentity = new ClaimsIdentity(claims, "cookie");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                userProfile.LastLogin = DateTime.UtcNow;
                await _homeHelperFunctions.UpdateUser(storageConfig, userProfile);

                if (userProfile.ProfileType == "tutor")
                    return Json(new { Message = JsonResponse.Tutor.ToString() });
                else
                    return Json(new { Message = JsonResponse.Student.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> EditProfileInformation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.User.Identity.Name;
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);

            //Handles if there is a profile picture with the caption or about paragraph
            if (Request.Form.Files.Count > 0)
            {
                var success = await _editProfileHelperFunctions.EditProfileWithProfilePicture(Request, storageConfig, userProfile, user);
                if (success != null)
                    return Json(new { Message = JsonResponse.Success.ToString(), caption = success[0], paragraph = success[1], picture = success[2]});
            }

            //Handles if there is not a profile picture with the caption or about paragraph
            if (Request.Form.Keys.Count == 1)
            {
                var success = await _editProfileHelperFunctions.EditProfileWithNoProfilePicture(Request, storageConfig, user);
                if (success != null)
                    return Json(new { Message = JsonResponse.Success.ToString(), caption = success[0], paragraph = success[1] });
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

            await _emailHelperFunctions.SendOutPasswordRecoveryEmail(userProfile, _homeHelperFunctions.CreateUri(userProfile.Username, key));  //send email out in thread so that delay will be shorter
            await _emailHelperFunctions.RunRecoveryEmailThread(storageConfig, userProfile, username);

            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string key)
        {
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if (!key.Equals(userProfile.ChangePasswordKey))
                return Json(new { Message = "Link Expired" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string newPassword, string confirmNewPassword, string username, string key) {
            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);

            if (!key.Equals(userProfile.ChangePasswordKey))
                return Json(new { Message = JsonResponse.QueryFailed.ToString() });

            if (newPassword == confirmNewPassword) {
                userProfile.Password = _homeHelperFunctions.EncryptPassword(newPassword);
                userProfile.ChangePasswordKey = null;
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
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
                GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
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
                TutorUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                GenericUserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SubscribeToNotifications([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email) //this is when the link is clicked from the settings page
        {
            if (email != null)
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUserUsingEmail, email);
                userProfile.NotificationSubscribe = DatabaseValues.True.ToString();
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email) //this is when the link is clicked from the email directly
        {
            if (email != null)
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUserUsingEmail, email);
                userProfile.NotificationSubscribe = DatabaseValues.False.ToString();
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UnsubscribeFromNotifications([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email) //this is when the link is clicked from the settings page
        {
            if (email != null)
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUserUsingEmail, email);
                userProfile.NotificationSubscribe = DatabaseValues.False.ToString();
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = JsonResponse.Success.ToString()});
            }

            return Json(new { Message = JsonResponse.Failed.ToString()});
        }
    }
}
