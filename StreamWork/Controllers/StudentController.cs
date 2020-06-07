using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.ViewModels;

namespace StreamWork.Controllers
{
    public class StudentController : Controller
    {
        readonly HomeMethods _homeMethods = new HomeMethods();
        readonly StudentMethods _studentMethods = new StudentMethods();
        readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();

        [HttpGet]
        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Student-ProfileStudent");

            var studentProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name);

            ProfileStudentViewModel viewModel = await _studentMethods.PopulateProfileStudentPage(storageConfig, HttpContext.User.Identity.Name);
            viewModel.StudentUserProfile = studentProfile;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LiveStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Student-ProfileStudent");

            var studentProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name);

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                LiveChannels = await _homeMethods.SearchUserChannels(storageConfig, s, q),
                StudentUserProfile = studentProfile,
                ArchivedStreams = await _homeMethods.GetAllArchivedStreams(storageConfig),
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, [FromQuery(Name = "s")] string s, [FromQuery(Name = "q")] string q)
        {
            // s is subject, q is search query
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Student-ArchivedStreams");

            string user = HttpContext.User.Identity.Name;

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                ArchivedStreams = await _homeMethods.SearchArchivedStreams(storageConfig, s, q),
                StudentUserProfile = user == null ? null : await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                StudentUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                ArchivedStreams = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithSubject, subject)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> StudentSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homeMethods._host + "/Home/Login?dest=-Student-StudentSettings");

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                StudentUserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
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
                var studentProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);

                if (_encryptionMethods.DecryptPassword(studentProfile.Password, currentPassword) == studentProfile.Password)
                {
                    studentProfile.Password = _encryptionMethods.EncryptPassword(newPassword);
                    await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", studentProfile.Id } }, studentProfile);
                    return Json(new { Message = JsonResponse.Success.ToString() });
                }
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCurrentAccount([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            UserLogin user = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, HttpContext.User.Identity.Name);
            if (user == null || user.ProfileType.Equals("Tutor"))
            {
                return Json(new { Message = JsonResponse.Failed.ToString() });
            }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await _studentMethods.DeleteUser(storageConfig, user);

            return Json(new { Message = JsonResponse.Success.ToString() });
        }
    }
}
