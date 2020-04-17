﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperClasses;
using StreamWork.ViewModels;

namespace StreamWork.Controllers
{
    public class StudentController : Controller
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        readonly StudentHelperFunctions _studentHelperFunctions = new StudentHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();

            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Student-ProfileStudent");

            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name);
            var splitName = userProfile.Name.Split(new char[] { '|' });
            model.FirstName = splitName[0];
            model.LastName = splitName[1];

            ProfileStudentViewModel viewModel = await _studentHelperFunctions.PopulateProfileStudentPage(storageConfig, HttpContext.User.Identity.Name);
            viewModel.UserProfile = userProfile;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LiveStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            var model = new UserProfile();

            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Student-ProfileStudent");

            var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name);
            var splitName = userProfile.Name.Split(new char[] { '|' });
            model.FirstName = splitName[0];
            model.LastName = splitName[1];

            return View(new ProfileStudentViewModel
            {
                LiveChannels = await _homeHelperFunctions.SearchUserChannels(storageConfig, s, q),
                UserProfile = userProfile,
                ArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos),
            });
        }

        [HttpGet]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            // s is subject, q is search query
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Student-ArchivedStreams");

            string user = HttpContext.User.Identity.Name;
            return View(new ProfileStudentViewModel
            {
                ArchivedStreams = await _homeHelperFunctions.SearchArchivedStreams(storageConfig, s, q),
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
            });
        }

        [HttpPost]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                ArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideosBasedOnSubject, subject)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> StudentSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Student-StudentSettings");

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                AllTutors = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> StudentSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string currentPassword,
                                                                                                                       string newPassword,
                                                                                                                       string confirmPassword)
        {
            if (currentPassword != null && newPassword != null && confirmPassword != null)
            {
                var user = HttpContext.User.Identity.Name;
                var userLogin = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user);

                if (_homeHelperFunctions.DecryptPassword(userLogin[0].Password, currentPassword) == userLogin[0].Password)
                {
                    userLogin[0].Password = _homeHelperFunctions.EncryptPassword(newPassword);
                    await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userLogin[0].Id } }, userLogin[0]);
                    return Json(new { Message = JsonResponse.Success.ToString() });
                }
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
