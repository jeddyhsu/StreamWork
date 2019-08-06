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

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public IActionResult Index()
        {
            if (Request.Host.ToString() == "streamwork.live")
            {
                return Redirect("https://www.streamwork.live");
            }
            return View();
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

        [HttpGet]
        public async Task<IActionResult> ProfileView(string Tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            ProfileTutorViewModel profile = new ProfileTutorViewModel
            {
                userArchivedVideos = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { Tutor }),
                userLogins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { Tutor })
            };
            return View(profile);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<ProfileTutorViewModel> PopulateSubjectPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var streams = await GetStreams(storageConfig, subject);
            var tutors = await GetPopularStreamTutors(storageConfig);
            ProfileTutorViewModel model = new ProfileTutorViewModel
            {
                userChannels = streams,
                userLogins = tutors
            };
            return model;
        }

        private async Task<List<UserLogin>> GetPopularStreamTutors([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            List<UserLogin> list = new List<UserLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            foreach (UserLogin user in getCurrentUsers)
            {
                if (user.ProfileType.Equals("tutor"))
                {
                    list.Add(user);
                }
            }
            return list;
        }

        private async Task<List<UserChannel>> GetStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var getAllStreams = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, "AllStreamKeys", new List<string> { subject });
            List<UserChannel> list = getAllStreams;
            return list;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string phone, string username, string password, string passwordConfirm, string channelId, string role)
        {
            string confirmation = "";
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

            UserChannel key = new UserChannel
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                ChannelKey = null,
                SubjectStreaming = null,
                StreamThumbnail = null,
                StreamTitle = null,
                VideoURL = null
            };
            var checkCurrentUsers = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { username });
            int numberOfUsers = 0;
            foreach (var user in checkCurrentUsers)
            {
                numberOfUsers++;
            }
            if (numberOfUsers != 0)
                confirmation = "Username already exsists";
            else if (password != passwordConfirm)
                confirmation = "Wrong Password";
            else
            {
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProflie.Id } }, signUpProflie);
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", key.Id } }, key);
                confirmation = "Success";
            }
            return Json(new { Message = confirmation });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string password)
        {
            string confirmation = "";
            if (storageConfig != null)
            {
                int user = 0;
                var checkforUser = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "AllSignedUpUsersWithPassword", new List<string> { username, password });
                foreach (var u in checkforUser)
                {
                    if (u.Password == password && u.Username == username)
                    {
                        user++;
                        if (u.ProfileType == "tutor")
                        {
                            user++;
                        }
                    }
                }
                if (user == 1)
                {
                    confirmation = "Welcome";
                    HttpContext.Session.SetString("UserProfile", username);
                }
                if (user == 2)
                {
                    confirmation = "Welcome, StreamTutor";
                    HttpContext.Session.SetString("UserProfile", username);
                }
            }
            else
            {
                confirmation = "Wrong Password or Username ";
            }
            return Json(new { Message = confirmation });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
