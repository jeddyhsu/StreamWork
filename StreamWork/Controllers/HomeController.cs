using System;
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

namespace StreamWork.Controllers
{
    public class HomeController : Controller {
        HelperFunctions helperFunctions = new HelperFunctions();

        public async Task<IActionResult> Index ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (Request.Host.ToString() == "streamwork.live") {
                return Redirect("https://www.streamwork.live");
            }
            var user = HttpContext.Session.GetString("UserProfile");
            if (user == null) {
                return View();
            }
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);
            return View(userProfile);
        }

        public async Task<IActionResult> Math ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Mathematics"));
        }

        public async Task<IActionResult> Science ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Science"));
        }

        public async Task<IActionResult> Engineering ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Engineering"));
        }

        public async Task<IActionResult> Business ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Business"));
        }

        public async Task<IActionResult> Law ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Law"));
        }

        public async Task<IActionResult> DesignArt ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Art"));
        }

        public async Task<IActionResult> Humanities ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Humanities"));
        }

        public async Task<IActionResult> Other ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return View(await PopulateSubjectPage(storageConfig, "Other"));
        }

        public IActionResult BecomeTutor () {
            return View();
        }

        public IActionResult About () {
            return View();
        }

        public IActionResult HowToStream () {
            return View();
        }

        public IActionResult SplashPage () {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView (string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileTutorViewModel profile = new ProfileTutorViewModel {
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user),
                userArchivedVideos = await helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, tutor),
                userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                userProfile2 = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
            };
            return View(profile);
        }

        public IActionResult Error () {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<ProfileTutorViewModel> PopulateSubjectPage ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject) {
            var user = HttpContext.Session.GetString("UserProfile");

            ProfileTutorViewModel model = new ProfileTutorViewModel {
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreamingWithSpecifiedSubject, subject),
                userLogins = await GetPopularStreamTutors(storageConfig),
                userProfile = user != null ? await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user) : null
            };
            return model;
        }

        private async Task<List<UserLogin>> GetPopularStreamTutors ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            List<UserLogin> list = new List<UserLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            foreach (UserLogin user in getCurrentUsers) {
                if (user.ProfileType.Equals("tutor") && user.AcceptedTutor) {
                    list.Add(user);
                }
            }
            return list;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string payPalAddress, string username, string password, string passwordConfirm, string role) {
            //Checks for the attachments that tutors provide and sends them to streamwork for verification
            if (Request.Form.Files.Count != 0) {
                List<string> values = new List<string>();
                foreach (var key in Request.Form.Keys) {
                    Request.Form.TryGetValue(key, out StringValues value);
                    values.Add(value);
                }

                nameFirst = values[0];
                nameLast = values[1];
                email = values[2];
                payPalAddress = values[3];
                username = values[4];
                password = values[5];
                passwordConfirm = values[6];
                role = values[7];

                var files = Request.Form.Files;
                List<Attachment> attachments = new List<Attachment>();
                foreach (var file in files) {
                    attachments.Add(new Attachment(file.OpenReadStream(), file.FileName));
                }
                helperFunctions.SendEmailToAnyEmail("streamworktutor@gmail.com", "streamworktutor@gmail.com", "Tutor Eval", email, attachments);
            }

            var checkCurrentUsers = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "CurrentUser", new List<string> { username });
            if (checkCurrentUsers.Count == 0) {
                if (password != passwordConfirm) {
                    return Json(new { Message = "Passwords do not match" });
                }

                UserLogin signUpProfile = new UserLogin {
                    Id = Guid.NewGuid().ToString(),
                    Name = nameFirst + "|" + nameLast,
                    EmailAddress = email,
                    Username = username,
                    Password = helperFunctions.EncryptPassword(password),
                    ProfileType = role,
                    AcceptedTutor = false,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                    Balance = (decimal)0f,
                    Expiration = DateTime.UtcNow,
                    TrialAccepted = false,
                    PayPalAddress = payPalAddress
                };
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProfile.Id } }, signUpProfile);

                if (role == "tutor") {
                    UserChannel userChannel = new UserChannel {
                        Id = Guid.NewGuid().ToString(),
                        Username = username,
                        ChannelKey = null,
                        StreamSubject = null,
                        StreamThumbnail = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                        StreamTitle = null,
                        ChatId = FormatChatId(DataStore.GetChatID("https://www.cbox.ws/apis/threads.php?id=6-829647-oq4rEn&key=ae1682707f17dbc2c473d946d2d1d7c3&act=mkthread"))
                    };
                    await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                }
                return Json(new { Message = JsonResponse.Success.ToString() });
            }
            return Json(new { Message = "Username already exists" });
        }

        [HttpGet]
        public IActionResult SignUp () {
            return View();
        }

        [HttpPost]
        public IActionResult TryLogin ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string placeholder) {
            try {
                HttpContext.Session.GetString("UserProfile");
                if (HttpContext.Session.GetString("Tutor").Equals("true")) {
                    return Json(new { Message = "Welcome, StreamTutor" });
                }
                return Json(new { Message = "Welcome" });
            }
            catch {
                return Json(new { Message = "Wrong Password or Username " });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string password) {
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if (userProfile == null)
                return Json(new { Message = "Error" });
            var checkforUser = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "AllSignedUpUsersWithPassword", new List<string> { username, helperFunctions.DecryptPassword(userProfile.Password, password) });
            if (checkforUser.Count == 1) {
                checkforUser[0].LoggedIn = "Logged In";
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", checkforUser[0].Id } }, checkforUser[0]);
                if (checkforUser[0].ProfileType == "tutor") {
                    HttpContext.Session.SetString("UserProfile", username);
                    HttpContext.Session.SetString("Tutor", "false");
                    return Json(new { Message = "Welcome, StreamTutor" });
                }
                else {
                    HttpContext.Session.SetString("UserProfile", username);
                    HttpContext.Session.SetString("Tutor", "false");
                    return Json(new { Message = "Welcome Student" });
                }
            }
            return Json(new { Message = "Wrong Password or Username" });
        }

        [HttpGet]
        public IActionResult Login () {
            return View();
        }

        public async Task<IActionResult> CreateDonationAttempt([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string studentName, string tutorName) {
            await helperFunctions.SaveDonationAttempt(storageConfig, new Models.DonationAttempt {
                Id = Guid.NewGuid().ToString(),
                Student = studentName,
                Tutor = tutorName,
                TimeSent = DateTime.UtcNow
            });
            return Json(new { Message = "Success" });
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
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if(userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString() });
            helperFunctions.SendEmailToAnyEmail("streamworktutor@gmail.com",userProfile.EmailAddress, "Password Recovery", helperFunctions.CreateUri(userProfile.Username),null);
            return Json(new { Message = JsonResponse.Success.ToString()});
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
                var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
                userProfile.Password = helperFunctions.EncryptPassword(newPassword);
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = JsonResponse.Success.ToString()});
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
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);
            userProfile.LoggedIn = null;
            await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
            return Json(new { Message = JsonResponse.Success.ToString()});
        }

        [HttpGet]
        public IActionResult Subscribe() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Donate (string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            var user = HttpContext.Session.GetString("UserProfile");

            ProfileTutorViewModel model = new ProfileTutorViewModel {
                userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                userProfile2 = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
            };
            return View(model);
        }

        public IActionResult PickStudentOrTutor()
        {
            return View();
        }
    }
}
