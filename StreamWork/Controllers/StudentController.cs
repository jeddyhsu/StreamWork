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
        HelperFunctions helperFunctions = new HelperFunctions();

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpGet]
        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);
            var splitName = userProfile.Name.Split(new char[] { '|' });
            model.FirstName = splitName[0];
            model.LastName = splitName[1];

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming, "NULL"),
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
                userArchivedStreams = await helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
                userArchivedStreams = await helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideosBasedOnSubject, subject)
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> StudentSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
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
                var user = HttpContext.Session.GetString("UserProfile");
                var userLogin = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user);
                if (helperFunctions.DecryptPassword(userLogin[0].Password, currentPassword) == userLogin[0].Password)
                {
                    userLogin[0].Password = helperFunctions.EncryptPassword(newPassword);
                    await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userLogin[0].Id } }, userLogin[0]);
                    return Json(new { Message = JsonResponse.Success.ToString() });
                }
            }
            return Json(new { Message = JsonResponse.Failed.ToString()});
        }
    }
}
