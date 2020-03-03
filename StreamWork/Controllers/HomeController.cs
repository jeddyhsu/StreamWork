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
using StreamWork.Models;

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
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming, subject),
                userLogins = await GetPopularStreamTutors(storageConfig),
                userProfile = user != null ? await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user) : null
            };
            return model;
        }

        private async Task<List<UserLogin>> GetPopularStreamTutors ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            List<UserLogin> list = new List<UserLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            foreach (UserLogin user in getCurrentUsers) {
                if (user.ProfileType.Equals("tutor")) {
                    list.Add(user);
                }
            }
            return list;
        }

        private async Task<List<Payment>> GetPayments ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return await DataStore.GetListAsync<Payment>(helperFunctions._connectionString, storageConfig.Value, "AllPayments");
        }

        private async Task<List<UserLogin>> GetStudents ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            List<UserLogin> list = new List<UserLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<UserLogin>(helperFunctions._connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            foreach (UserLogin user in getCurrentUsers) {
                if (user.ProfileType.Equals("student")) {
                    list.Add(user);
                }
            }
            return list;
        }

        private async Task<List<DonationAttempt>> GetDonationAttempts ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            List<DonationAttempt> list = await DataStore.GetListAsync<DonationAttempt>(helperFunctions._connectionString, storageConfig.Value, "AllDonationAttempts");
            return list;
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

        [HttpGet]
        public IActionResult Login () {
            return View();
        }

        private string FormatChatId (string chatID) {
            var formattedphrase = chatID.Split(new char[] { '\t' });
            var formattedChatID = formattedphrase[2].Split(new char[] { '\n' });
            return formattedphrase[1] + "|" + formattedChatID[0];
        }

        [HttpGet]
        public IActionResult PasswordRecovery () {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username) {
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if (userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString() });
            helperFunctions.SendEmailToAnyEmail(userProfile.EmailAddress, "Password Recovery", helperFunctions.CreateUri(userProfile.Username));
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public IActionResult ChangePassword () {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string newPassword, string confirmNewPassword, string path) {
            if (newPassword == confirmNewPassword) {
                var pathFormat = path.Split(new char[] { '=' });
                var username = pathFormat[1];
                var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
                userProfile.Password = newPassword;
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = JsonResponse.Success.ToString() });
            }
            return Json(new { Message = "Invalid Password Match" });
        }

        [HttpGet]
        public IActionResult Logout () {
            return View();
        }

        [HttpGet]
        public IActionResult Subscribe () {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ControlPanel ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            var students = await GetStudents(storageConfig);
            var tutors = await GetPopularStreamTutors(storageConfig);
            var payments = await GetPayments(storageConfig);
            return View(new ControlPanelViewModel {
                Students = students,
                Tutors = tutors,
                Payments = payments
            });;
        }

        [HttpPost]
        public async Task<IActionResult> AcceptTutor ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username) {
            var user = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            user.AcceptedTutor = true;
            await helperFunctions.UpdateUser(storageConfig, user);
            return Json(new { Message = "Success" });
        }

        [HttpPost]
        public async Task<IActionResult> ZeroTutorBalance ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username) {
            var user = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            user.Balance = 0;
            await helperFunctions.UpdateUser(storageConfig, user);
            return Json(new { Message = "Success" });
        }

        [HttpPost]
        public async Task<IActionResult> RenewStudentSubscription ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username) {
            var user = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            user.Expiration = DateTime.UtcNow.AddMonths(1);
            user.TrialAccepted = true;
            await helperFunctions.UpdateUser(storageConfig, user);
            return Json(new { Message = "Success" });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyDonationAttempt ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id, string value) {
            if (decimal.TryParse(value, out decimal decimalValue)) {
                var donationAttempt = await helperFunctions.GetDonationAttempt(storageConfig, "DonationAttemptsById", id);
                var tutor = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, donationAttempt.Tutor);
                tutor.Balance += decimalValue;
                await helperFunctions.UpdateUser(storageConfig, tutor);
                await helperFunctions.DeleteDonationAttempt(storageConfig, donationAttempt);
                return Json(new { Message = "Success" });
            }
            return Json(new { Message = "Failure" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDonationAttempt ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id) {
            var donationAttempt = await helperFunctions.GetDonationAttempt(storageConfig, "DonationAttemptsById", id);
            await helperFunctions.DeleteDonationAttempt(storageConfig, donationAttempt);
            return Json(new { Message = "Success" });
        }

        [HttpPost]
        public async Task<IActionResult> Test ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            await helperFunctions.SaveDonationAttempt(storageConfig, new DonationAttempt {
                Id = Guid.NewGuid().ToString(),
                Student = "tom",
                Tutor = "rarunT",
                TimeSent = DateTime.UtcNow
            });
            return Json(new { Message = "Success" });
        }
    }
}
