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
using System.Data.Common;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";

        private bool checker;

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

        private async Task<UserChannel> GetUserChannelInfo([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var channel = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { HttpContext.Session.GetString("UserProfile") });
            return channel[0];
        }

        private async Task<bool> SaveIntoBlobContainer(IFormFile file, string profileCaption, string profileParagraph, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            //Gets users channel
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });

            //Connects to blob storage and saves thumbnail from user
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(profileCaption);


            using(MemoryStream ms = new MemoryStream())
            {
                try
                {
                    await file.CopyToAsync(ms);
                    blockBlob.UploadFromByteArray(ms.ToArray(), 0, (int)file.Length);
                }
                catch(System.ObjectDisposedException e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
           
            //Populates stream title and stream thumbnail url and saves it into sql database
            getUserInfo[0].ProfileCaption = profileCaption;
            getUserInfo[0].ProfilePicture = blockBlob.Uri.AbsoluteUri;
            getUserInfo[0].ProfileParagraph = profileParagraph;
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", getUserInfo[0].Id } }, getUserInfo[0]);

            return true;
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
                ProfileType = role
            };
            var _streamKey = channelId.Replace('C', 'U');
            UserChannel key = new UserChannel
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                ChannelKey = _streamKey,
                ChannelKeyAPI = channelId,
                SubjectStreaming = null,
                StreamThumbnail = null
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
            var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });
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
            var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });
            var userChannel = await GetUserChannelInfo(storageConfig);
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
                if (userChannel != null)
                {
                    model.ChannelId = userChannel.ChannelKey;
                }
            }
            if (userChannel.StreamID == null && checker == false)
            {
                userChannel.SubjectStreaming = null;
                userChannel.StreamThumbnail = null;
                userChannel.StreamID = null;
                checker = true;
            }

            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
           // PopulateTutorPage(storageConfig);
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            { 
                userProfile = model,
                userLogins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user }),
                userChannels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { user }),
                userArchivedVideos = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { user })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamURL, string streamTitle, string streamSubject, string subject, string stop, string change)
        {
            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                var file = Request.Form.Files[i];
                var fileSplit = file.Name.Split(new char[] { '|' });
                var profileCaption = fileSplit[0];
                var profileParagraph = fileSplit[1];
                await SaveIntoBlobContainer(file, profileCaption, profileParagraph, storageConfig);
                return Json(new { Message = "Success" });
            }

            if(Request.Form.Keys.Count > 0)
            {
                var user = HttpContext.Session.GetString("UserProfile");
                var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });

                var split1 = "";
                var split2 = "";
                foreach(string s in Request.Form.Keys)
                {
                    var array = s.Split(new char[] { '|' });
                    split1 = array[0];
                    split2 = array[1];
                    break;
                }

                getUserInfo[0].ProfileCaption = split1;
                getUserInfo[0].ProfileParagraph = split2;

                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", getUserInfo[0].Id } }, getUserInfo[0]);
                return Json(new { Message = "Success" });
            }

            var userChannel = await GetUserChannelInfo(storageConfig);

            //Saves streamTitle, URl, and subject into sql database
            if(streamURL != null && streamTitle != null && streamSubject != null)
            {
                var streamId = streamURL.Split(new char[] { '/' });
                userChannel.StreamID = streamId[3] ;
                userChannel.VideoURL = streamURL;
                userChannel.SubjectStreaming = streamSubject;
                userChannel.StreamThumbnail = "https://i.ytimg.com/vi/"+streamId[3]+"/hqdefault_live.jpg";
                userChannel.StreamTitle = streamTitle;

                try
                {
                    await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                }
                catch (DbException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return Json(new {Message = "Saved"});
            }
            //change stream subject
            if (change != null)
            {
                userChannel.SubjectStreaming = change;
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
            }
            //stop stream and archvie video into database
            if (stop != null)
            {
                UserArchivedStreams video = new UserArchivedStreams
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = userChannel.Username,
                    StreamID = userChannel.StreamID,
                    StreamThumbnail = userChannel.StreamThumbnail,
                    Subject = "https://i.ytimg.com/vi/"+ userChannel.StreamID + "/hqdefault.jpg",
                    StreamTitle = userChannel.StreamTitle,
                };

                userChannel.SubjectStreaming = null;
                userChannel.StreamThumbnail = null;
                userChannel.StreamID = null;
                userChannel.StreamTitle = null;
                userChannel.VideoURL = null;

                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", video.Id } }, video);
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);

                return Json(new { Message = "Stopped" });
            }

            return Json(new { Message = "" });
        }
    }
}
