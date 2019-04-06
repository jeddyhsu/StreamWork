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


        private bool checker;

        public IActionResult Index()
        {
            if(Request.Host.ToString() == "streamwork.live")
            {
                return Redirect("https://www.streamwork.live");
            }
            return View();
        }

        public async Task<IActionResult> Math([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig,"Math"));
        }

        public async Task<IActionResult> Literature([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig,"Literature"));
        }

        public async Task<IActionResult> Engineering([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Engineering"));
        }

        public async Task<IActionResult> DesignArt([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "DesignArt"));
        }

        public async Task<IActionResult> Science([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Science"));
        }

        public async Task<IActionResult> Business([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Business"));
        }

        public async Task<IActionResult> Programming([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Programming"));
        }

        public async Task<IActionResult> Other([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Other"));
        }

        public IActionResult BecomeTutor()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView(string Tutor,[FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            ProfileViewModel profile = new ProfileViewModel
            {
                archivedStreams = await DataStore.GetListAsync<ArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { Tutor }),
                users = await DataStore.GetListAsync<StreamWorkLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { Tutor })
            };
            return View(profile);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async void PopulateTutorPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var userChannel = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { HttpContext.Session.GetString("UserProfile") });
        }

        private async Task<SubjectViewModel> PopulateSubjectPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var streams = await GetStreams(storageConfig, subject);
            var tutors = await GetPopularStreamTutors(storageConfig);
            SubjectViewModel model = new SubjectViewModel();
            model.streamList = streams;
            model.streamTutorList = tutors;
            return model;
        }

        private async Task<List<StreamWorkLogin>> GetPopularStreamTutors([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            List<StreamWorkLogin> list = new List<StreamWorkLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<StreamWorkLogin>(_connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            list = getCurrentUsers;
            return list;
        }

        private async Task<List<YoutubeChannelID>> GetStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            List<YoutubeChannelID> list = new List<YoutubeChannelID>();
            var getAllStreams = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "AllStreamKeys", new List<string> { subject });
            list = getAllStreams;
            return list;
        }

        private async Task<bool> CallAPIForConnectingToLiveStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, List<YoutubeChannelID> userChannel, string subject)
        {
            bool b = true;
            while (b)
            {
                await Task.Delay(5000);
                var API = DataStore.CallAPI("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=" + userChannel[0].ChannelKeyAPI + "&type=video&eventType=live&key=AIzaSyAk_G6WQE-W9gldyF_CvNLLHaSs4psLuIA");
                if (API.Items.Length == 1)
                {
                    userChannel[0].SubjectStreaming = subject;
                    userChannel[0].StreamThumbnail = API.Items[0].Snippet.Thumbnails.High.Url.ToString();
                    userChannel[0].StreamID = API.Items[0].Id.VideoId;
                    await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                    b = false;
                }
            }
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string phone, string username, string password, string passwordConfirm, string channelId, string role)
        {
            string confirmation = "";
            StreamWorkLogin signUpProflie = new StreamWorkLogin
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameFirst + "|" + nameLast,
                EmailAddress = email,
                Username = username,
                Password = password,
                ProfileType = role
            };
            var _streamKey = channelId.Replace('C', 'U');
            YoutubeChannelID key = new YoutubeChannelID
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                ChannelKey = _streamKey,
                ChannelKeyAPI = channelId,
                SubjectStreaming = null,
                StreamThumbnail = null
            };
            var checkCurrentUsers = await DataStore.GetListAsync<StreamWorkLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { username });
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
                var checkforUser = await DataStore.GetListAsync<StreamWorkLogin>(_connectionString, storageConfig.Value, "AllSignedUpUsersWithPassword", new List<string> { username, password });
                foreach (var u in checkforUser)
                {
                    if (u.Password == password && u.Username == username)
                    {
                        user++;
                    }
                }
                if (user == 1)
                {
                    confirmation = "Welcome";
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

        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<StreamWorkLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<StreamWorkLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });
            var userChannel = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { user });
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
                if (userChannel.ToArray().Length != 0)
                {
                    model.ChannelId = userChannel[0].ChannelKey;
                }
            }
            if (userChannel[0].StreamID == null && checker == false)
            {
                userChannel[0].SubjectStreaming = null;
                userChannel[0].StreamThumbnail = null;
                userChannel[0].StreamID = null;
                checker = true;
            }

            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
            PopulateTutorPage(storageConfig);
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userProfile = model,
                youtubeStreamKeys = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { user }),
                archivedVideos = await DataStore.GetListAsync<ArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { user })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamKey, string subject, string stop, string change)
        {
            string confirmation = "";

            if (subject != null)
            {
                var userChannel = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { HttpContext.Session.GetString("UserProfile") });

                if (await CallAPIForConnectingToLiveStream(storageConfig, userChannel, subject))
                {
                    confirmation = "Success";
                }
            }

            if (change != null)
            {
                var userChannel = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { HttpContext.Session.GetString("UserProfile") });
                userChannel[0].SubjectStreaming = change;
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                confirmation = "Success";
            }

            if (stop != null)
            {
                var userChannel = await DataStore.GetListAsync<YoutubeChannelID>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { HttpContext.Session.GetString("UserProfile") });

                ArchivedStreams video = new ArchivedStreams
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = HttpContext.Session.GetString("UserProfile"),
                    StreamID = userChannel[0].StreamID,
                    StreamThumbnail = userChannel[0].StreamThumbnail,
                    Subject = userChannel[0].SubjectStreaming
                };

                userChannel[0].SubjectStreaming = null;
                userChannel[0].StreamThumbnail = null;
                userChannel[0].StreamID = null;

                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", video.Id } }, video);
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);

                confirmation = "Success";
            }
            return Json(new { Message = confirmation });
        }
    }
}
