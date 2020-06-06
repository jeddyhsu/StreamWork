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
        readonly HomeMethods _homeMethods = new HomeMethods();
        readonly TutorMethods _tutorMethods = new TutorMethods();
        readonly FollowingMethods _followingMethods = new FollowingMethods();
        readonly EmailMethods _emailMethods = new EmailMethods();
        readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();
        readonly EditProfileMethods _editProfileMethods = new EditProfileMethods();

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await _homeMethods.PopulateHomePage(storageConfig, HttpContext.User.Identity.Name, HttpContext.User.Identity.IsAuthenticated));
        }

        public async Task<IActionResult> Search([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            // s is subject, q is search query
            string user = HttpContext.User.Identity.Name;
            SearchViewModel viewModel = new SearchViewModel
            {
                PopularStreamTutors = await _homeMethods.GetPopularStreamTutor(storageConfig),
                StreamResults = await _homeMethods.SearchUserChannels(storageConfig, s, q),
                ArchiveResults = await _homeMethods.SearchArchivedStreams(storageConfig, s, q),
                UserProfile = user == null ? null : await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user),
                Subject = string.IsNullOrEmpty(s) ? "All Subjects" : s,
                SearchQuery = q,
                SubjectIcon = _homeMethods.GetSubjectIcon(s)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> BecomeTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            PopularStreamTutorsViewModel viewModel;
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                viewModel = new PopularStreamTutorsViewModel
                {
                    GenericUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, HttpContext.User.Identity.Name)
                };
            }
            else viewModel = new PopularStreamTutorsViewModel { };

            viewModel.PopularStreamTutors = await _homeMethods.GetPopularStreamTutor(storageConfig);

            return View(viewModel);
        }

        public async Task<IActionResult> About([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
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

        public async Task<IActionResult> PickStudentOrTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
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

        public IActionResult SplashPage()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string tutor)
        {
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                GenericUserProfile = HttpContext.User.Identity.Name != null ? await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, HttpContext.User.Identity.Name) : null,
                TutorUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, tutor),
                UserChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, tutor),
                UserArchivedVideos = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, tutor),
                NumberOfStreams = (await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, tutor)).Count
            };

            viewModel.NumberOfFollowers = await _followingMethods.GetNumberOfFollowers(storageConfig, viewModel.TutorUserProfile.Id);
            if (HttpContext.User.Identity.IsAuthenticated) viewModel.IsFollowing = await _followingMethods.IsFollowingFollowee(storageConfig, viewModel.GenericUserProfile.Id, viewModel.TutorUserProfile.Id);
            viewModel.Schedule = _tutorMethods.GetTutorStreamSchedule(viewModel.UserChannel);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if (await _followingMethods.AddFollower(storageConfig, followerId, followeeId)) return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if (await _followingMethods.RemoveFollower(storageConfig, followerId, followeeId)) return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecommendation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string student, string tutor, string recommendation)
        {
            await _homeMethods.SaveRecommendation(storageConfig, student, tutor, recommendation);
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string nameFirst, string nameLast, string email, string payPalAddress, string username, string password, string college, string role)
        {

            if (password == null && email != null) //initial checks!
            {
                var checkUsername = await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetUserWithUsername, username);
                if (checkUsername.Count != 0) return Json(new { Message = JsonResponse.UsernameExists.ToString() });

                var checkEmail = await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetUserWithEmailAddress, email);
                if (checkEmail.Count != 0) return Json(new { Message = JsonResponse.EmailExists.ToString() });

                if (payPalAddress != null) //only for tutors
                {
                    var checkPayPalEmailUsingRegularEmail = await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetUserWithEmailAddress, payPalAddress); //payPal email can't be someone elses regular email
                    var checkPayPalEmail = await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetUserWithPayPalAddress, payPalAddress);
                    if (checkPayPalEmailUsingRegularEmail.Count != 0 || checkPayPalEmail.Count != 0) return Json(new { Message = JsonResponse.PayPalEmailExists.ToString() });
                }

                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            UserLogin userProfile = new UserLogin
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameFirst + "|" + nameLast,
                EmailAddress = email,
                Username = username,
                Password = _encryptionMethods.EncryptPassword(password),
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

            if (userProfile.ProfileType == "student") await _emailMethods.SendOutEmailToStreamWorkTeam(userProfile);
            await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            if (role == "tutor")
            {
                //Create User Channel For Tutor
                UserChannel userChannel = new UserChannel
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = username,
                    ChannelKey = _homeMethods._defaultStreamHosterChannelKey,
                    StreamSubject = null,
                    StreamThumbnail = null,
                    StreamTitle = null,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                };
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);

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
                    await _emailMethods.SendOutEmailToStreamWorkTeam(nameFirst, nameLast, email, attachments);
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
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, username);
            if (userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString() });

            var checkforUser = await DataStore.GetListAsync<UserLogin>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetUserWithUsernameAndPassword.ToString(), new List<string> { username, _encryptionMethods.DecryptPassword(userProfile.Password, password) });
            if (checkforUser.Count == 1)
            {
                HttpContext.Session.SetString(SQLQueries.UserProfile.ToString(), username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, userProfile.EmailAddress),
                    new Claim(ClaimTypes.UserData, _homeMethods.GetRandomChatColor())
                };

                var userIdentity = new ClaimsIdentity(claims, "cookie");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                userProfile.LastLogin = DateTime.UtcNow;
                await _homeMethods.UpdateUser(storageConfig, userProfile);

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
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
            var success = await _editProfileMethods.EditProfile(storageConfig, Request, userProfile);
            if (success != null) return Json(new { Message = JsonResponse.Success.ToString(), caption = success[0], paragraph = success[1], picture = success[2] });
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
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, username);
            if (userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString() });

            Random random = new Random();
            string key = Convert.ToString(random.Next(int.MaxValue), 16);
            userProfile.ChangePasswordKey = key;
            await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            await _emailMethods.SendOutPasswordRecoveryEmail(userProfile, _homeMethods.CreateUri(userProfile.Username, key));  //send email out in thread so that delay will be shorter
            await _emailMethods.RunRecoveryEmailThread(storageConfig, userProfile, username);

            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string key)
        {
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, username);
            if (!key.Equals(userProfile.ChangePasswordKey))
                return Json(new { Message = "Link Expired" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string newPassword, string confirmNewPassword, string username, string key)
        {
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, username);

            if (!key.Equals(userProfile.ChangePasswordKey))
                return Json(new { Message = JsonResponse.QueryFailed.ToString() });

            if (newPassword == confirmNewPassword)
            {
                userProfile.Password = _encryptionMethods.EncryptPassword(newPassword);
                userProfile.ChangePasswordKey = null;
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

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
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Home-Subscribe");

            var user = HttpContext.User.Identity.Name;

            DefaultViewModel viewModel = new DefaultViewModel
            {
                GenericUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Donate(string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Home-Donate?tutor=" + tutor);

            var user = HttpContext.User.Identity.Name;

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                TutorUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, tutor),
                GenericUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user)
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> SubscribeToNotifications([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email) //this is when the link is clicked from the settings page
        {
            if (email != null)
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithEmailAddress, email);
                userProfile.NotificationSubscribe = DatabaseValues.True.ToString();
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email) //this is when the link is clicked from the email directly
        {
            if (email != null)
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithEmailAddress, email);
                userProfile.NotificationSubscribe = DatabaseValues.False.ToString();
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UnsubscribeFromNotifications([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email) //this is when the link is clicked from the settings page
        {
            if (email != null)
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithEmailAddress, email);
                userProfile.NotificationSubscribe = DatabaseValues.False.ToString();
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = JsonResponse.Success.ToString() });
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
