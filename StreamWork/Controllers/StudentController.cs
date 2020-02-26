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
        HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        StudentHelperFunctions _studentHelperFunctions = new StudentHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
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

            ProfileStudentViewModel viewModel = await _studentHelperFunctions.PopulateStudentLiveStreamsPage(storageConfig, s, q, HttpContext.User.Identity.Name);
            viewModel.UserProfile = userProfile;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            // s is subject, q is search query
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Student-ArchivedStreams");

            return View(await _studentHelperFunctions.PopulateArchivePage(storageConfig, s, q, HttpContext.User.Identity.Name));
        }

        [HttpPost]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var user = HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString());

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
                UserArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideosBasedOnSubject, subject)
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
                UserLogins = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name),
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
                var user = HttpContext.Session.GetString(QueryHeaders.UserProfile.ToString());
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
