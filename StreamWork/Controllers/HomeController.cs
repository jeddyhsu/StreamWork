﻿using System;
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

namespace StreamWork.Controllers
{
    public class HomeController : Controller {

        readonly HomeHelperFunctions _homehelperFunctions = new HomeHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> Index ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);
                return View(userProfile);
            }

            return View();
        }
        
        public async Task<IActionResult> Math ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Math");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Mathematics", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> Science ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Science");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Science", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> Engineering ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Engineering");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Engineering", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> Business ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Business");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Business", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> Law ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Law");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Law", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> DesignArt ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Art");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Art", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> Humanities ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Humanities");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Humanities", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> Other ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Other");

            return View(await _homehelperFunctions.PopulateSubjectPage(storageConfig, "Other", HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString())));
        }

        public async Task<IActionResult> BecomeTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {

            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);
                return View(userProfile);
            }

            return View();
        }

        public async Task<IActionResult> About([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);
                return View(userProfile);
            }

            return View();
        }

        public async Task<IActionResult> HowToStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name);
                return View(userProfile);
            }

            return View();
        }

        public async Task<IActionResult> PickStudentOrTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString());
            if (user == null)
                return View();

            var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);

            return View(userProfile);
        }

        public IActionResult SplashPage () {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView (string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {

            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-ProfileView");

            ProfileTutorViewModel profile = new ProfileTutorViewModel {

                userChannels = await _homehelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, User.Identity.Name),
                userArchivedVideos = await _homehelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, tutor),
                userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                studentOrtutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name)
            };

            return View(profile);
        }

        public IActionResult Error () {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string payPalAddress, string username, string password, string passwordConfirm, string role) {

            var checkCurrentUsers = await DataStore.GetListAsync<UserLogin>(_homehelperFunctions._connectionString, storageConfig.Value, "CurrentUser", new List<string> { username });
            if (checkCurrentUsers.Count == 0)
            {
                UserLogin signUpProfile = new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = nameFirst + "|" + nameLast,
                    EmailAddress = email,
                    Username = username,
                    Password = _homehelperFunctions.EncryptPassword(password),
                    ProfileType = role,
                    AcceptedTutor = false,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                    Balance = (decimal)0f,
                    Expiration = DateTime.UtcNow,
                    TrialAccepted = false,
                    PayPalAddress = payPalAddress
                };
                await DataStore.SaveAsync(_homehelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProfile.Id } }, signUpProfile);

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
                        ChatId = _homehelperFunctions.FormatChatId(DataStore.GetChatID("https://www.cbox.ws/apis/threads.php?id=6-829647-oq4rEn&key=ae1682707f17dbc2c473d946d2d1d7c3&act=mkthread"))
                    };
                    await DataStore.SaveAsync(_homehelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);

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
                        await _homehelperFunctions.SendEmailToAnyEmailAsync(_homehelperFunctions._streamworkEmailID, _homehelperFunctions._streamworkEmailID, "Tutor Evaluation", email, attachments);
                    }
                }
            }
            else
            {
                return Json(new { Message = JsonResponse.Failed.ToString()});
            }

            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public IActionResult SignUp () {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string password) {
            var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if (userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString()});

            var checkforUser = await DataStore.GetListAsync<UserLogin>(_homehelperFunctions._connectionString, storageConfig.Value, QueryHeaders.AllSignedUpUsersWithPassword.ToString(), new List<string> { username, _homehelperFunctions.DecryptPassword(userProfile.Password, password)});
            if (checkforUser.Count == 1) {

                HttpContext.Session.SetString(QueryHeaders.UserProfile.ToString(), username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, userProfile.EmailAddress),
                };

                var userIdentity = new ClaimsIdentity(claims, "cookie");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                if(userProfile.ProfileType == "tutor")
                    return Json(new { Message = JsonResponse.Tutor.ToString()});
                else
                    return Json(new { Message = JsonResponse.Student.ToString()});
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public IActionResult Login () {
            return View();
        }

        public async Task<IActionResult> CreateDonationAttempt([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string studentName, string tutorName) {
            await _homehelperFunctions.SaveDonationAttempt(storageConfig, new Models.DonationAttempt {
                Id = Guid.NewGuid().ToString(),
                Student = studentName,
                Tutor = tutorName,
                TimeSent = DateTime.UtcNow
            });

            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public IActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username)
        {
            var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
            if(userProfile == null)
                return Json(new { Message = JsonResponse.Failed.ToString()});

            await _homehelperFunctions.SendEmailToAnyEmailAsync(_homehelperFunctions._streamworkEmailID, userProfile.EmailAddress, "Password Recovery", _homehelperFunctions.CreateUri(userProfile.Username),null);
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
                var userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);

                userProfile.Password = _homehelperFunctions.EncryptPassword(newPassword);
                await DataStore.SaveAsync(_homehelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

                return Json(new { Message = JsonResponse.Success.ToString()});
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
         }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string logout)
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Redirect(_homehelperFunctions._host + "/Home/Login?dest=-Home-Profile");
            return Json(new { Message = JsonResponse.Success.ToString() });
        }

        [HttpGet]
        public IActionResult Subscribe() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Donate (string tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            var user = HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString());

            ProfileTutorViewModel model = new ProfileTutorViewModel {
                userProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor),
                studentOrtutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user)
            };

            return View(model);
        }
    }
}
