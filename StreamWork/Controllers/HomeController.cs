using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using StreamWork.ViewModels;
using StreamWork.DataModels;
using StreamWork.HelperClasses;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {
        HelperFunctions helperFunctions = new HelperFunctions();

        public async Task<IActionResult> Index([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (Request.Host.ToString() == "streamwork.live")
            {
                return Redirect("https://www.streamwork.live");
            }
            var user = HttpContext.Session.GetString("UserProfile");
            if(user == null)
            {
                return View();
            }
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", user);
            return View(userProfile);
        }

        public async Task<IActionResult> Math([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Mathematics"));
        }

        public async Task<IActionResult> Science([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Science"));
        }

        public async Task<IActionResult> Engineering([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Engineering"));
        }

        public async Task<IActionResult> Business([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Business"));
        }

        public async Task<IActionResult> Law([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Law"));
        }

        public async Task<IActionResult> DesignArt([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Art"));
        }

        public async Task<IActionResult> Humanities([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Humanities"));
        }

        public async Task<IActionResult> Other([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Other"));
        }

        public IActionResult BecomeTutor()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult HowToStream()
        {
            return View();
        }

        public IActionResult SplashPage()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView(string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileTutorViewModel profile = new ProfileTutorViewModel
            {
                userChannels = await helperFunctions.GetUserChannels(storageConfig, "CurrentUserChannel", user),
                userArchivedVideos = await helperFunctions.GetArchivedStreams(storageConfig, "UserArchivedVideos", tutor),
                userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", tutor),
                userProfile2 = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", user)
            };
            return View(profile);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<ProfileTutorViewModel> PopulateSubjectPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var user = HttpContext.Session.GetString("UserProfile");

            ProfileTutorViewModel model = new ProfileTutorViewModel
            {
                userChannels = await helperFunctions.GetUserChannels(storageConfig, "AllUserChannelsThatAreStreaming", subject),
                userLogins = await GetPopularStreamTutors(storageConfig),
                userProfile = user != null ? await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", user) : null
            };
            return model;
        }

        private async Task<List<UserLogin>> GetPopularStreamTutors([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            List<UserLogin> list = new List<UserLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            foreach (UserLogin user in getCurrentUsers)
            {
                if (user.ProfileType.Equals("tutor"))
                {
                    list.Add(user);
                }
            }
            return list;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string username, string password, string passwordConfirm, string role)
        {
            var checkCurrentUsers = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "CurrentUser", new List<string> { username });
            if (checkCurrentUsers.Count >= 1)
            {
                if (password != passwordConfirm)
                {
                    return Json(new { Message = "Passwords do not match" });
                }

                UserLogin signUpProflie = new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = nameFirst + "|" + nameLast,
                    EmailAddress = email,
                    Username = username,
                    Password = password,
                    ProfileType = role,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png"
                };
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProflie.Id } }, signUpProflie);

                if (role == "tutor")
                {
                    UserChannel userChannel = new UserChannel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Username = username,
                        ChannelKey = null,
                        StreamSubject = null,
                        StreamThumbnail = null,
                        StreamTitle = null,
                        ChatId = FormatChatId(DataStore.GetChatID("https://www.cbox.ws/apis/threads.php?id=6-829647-oq4rEn&key=ae1682707f17dbc2c473d946d2d1d7c3&act=mkthread"))
                    };
                    await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                }
            }
             return Json(new { Message = "Username already exsists" });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TryLogin([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string placeholder)
        {
            try
            {
                HttpContext.Session.GetString("UserProfile");
                if (HttpContext.Session.GetString("Tutor").Equals("true"))
                {
                    return Json(new { Message = "Welcome, StreamTutor" });
                }
                return Json(new { Message = "Welcome" });
            }
            catch
            {
                return Json(new { Message = "Wrong Password or Username " });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string password)
        {
            var checkforUser = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "AllSignedUpUsersWithPassword", new List<string> { username, password });
            if (checkforUser.Count == 1)
            {
                checkforUser[0].LoggedIn = "Logged In";
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", checkforUser[0].Id } }, checkforUser[0]);
                if (checkforUser[0].ProfileType == "tutor")
                {
                    HttpContext.Session.SetString("UserProfile", username);
                    HttpContext.Session.SetString("Tutor", "false");
                    return Json(new { Message = "Welcome, StreamTutor" });
                }
                else
                {
                    HttpContext.Session.SetString("UserProfile", username);
                    HttpContext.Session.SetString("Tutor", "false");
                    return Json(new { Message = "Welcome Student" });
                }
            }
            return Json(new { Message = "Wrong Password or Username" });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        private string FormatChatId(string chatID)
        {
            var formattedphrase = chatID.Split(new char[] { '\t' });
            var formattedChatID = formattedphrase[2].Split(new char[] { '\n' });
            return formattedphrase[1] + "|" + formattedChatID[0];
        }

        [HttpGet]
        public IActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username)
        {
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", username);
            helperFunctions.SendEmailToAnyEmail(userProfile.EmailAddress, "Password Recovery", helperFunctions.CreateUri(userProfile.Username));
            return Json(new { Message = "Success"});
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string newPassword, string confirmNewPassword, string path)
        {
            if (newPassword == confirmNewPassword)
            {
                var pathFormat = path.Split(new char[] { '=' });
                var username = pathFormat[1];
                var userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", username);
                userProfile.Password = newPassword;
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = "Success" });
            }   
            return Json(new { Message = "Invalid Password Match" });
         }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string logout)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", user);
            userProfile.LoggedIn = null;
            await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
            return Json(new { Message = "Success" });
        }
    }
}
