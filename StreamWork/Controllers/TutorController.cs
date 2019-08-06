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
using System.IO;
using System;
using System.Data.Common;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace StreamWork.Controllers
{
    public class TutorController : Controller
    {
        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";

        private bool checker;

        [HttpGet]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userLogins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user }),
                userChannels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, "UserChannelKey", new List<string> { user }),
                userArchivedVideos = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { user })
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamTitle, string streamSubject, string change, string channelKey)
        {
            var userChannel = await GetUserChannelInfo(storageConfig);

            if(channelKey != null)
            {
                var channelInfo = DataStore.CallAPI("http://api.dacast.com/v2/channel/+"+channelKey+"?apikey=135034_bea5e11ca516995572c8&_format=JSON");

            }


            //Saves streamTitle, URl, and subject into sql database
            if (streamTitle != null && streamSubject != null)
            {
                userChannel.SubjectStreaming = streamSubject;
                userChannel.StreamTitle = streamTitle;
                try
                {
                    await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                }
                catch (DbException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return Json(new { Message = "Saved" });
            }
            //change stream subject
            if (change != null)
            {
                userChannel.SubjectStreaming = change;
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                return Json(new { Message = "Saved" });
            }

            return Json(new { Message = "Failed" });
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
            //if (userChannel.StreamID == null && checker == false)
            //{
            //    userChannel.SubjectStreaming = null;
            //    userChannel.StreamThumbnail = null;
            //    userChannel.StreamID = null;
            //    checker = true;
            //}

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

                var user = HttpContext.Session.GetString("UserProfile");
                var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });

                var split1 = "";
                var split2 = "";
                foreach (string s in Request.Form.Keys)
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

            if (Request.Form.Keys.Count > 0)
            {
                var user = HttpContext.Session.GetString("UserProfile");
                var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });

                var split1 = "";
                var split2 = "";
                foreach (string s in Request.Form.Keys)
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

            
            //stop stream and archvie video into database
            if (stop != null)
            {
                UserArchivedStreams video = new UserArchivedStreams
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = userChannel.Username,
                    StreamThumbnail = userChannel.StreamThumbnail,
                    StreamTitle = userChannel.StreamTitle,
                };

                userChannel.SubjectStreaming = null;
                userChannel.StreamThumbnail = null;
                userChannel.StreamTitle = null;
                userChannel.VideoURL = null;

                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", video.Id } }, video);
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);

                return Json(new { Message = "Stopped" });
            }

            return Json(new { Message = "" });
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


            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    await file.CopyToAsync(ms);
                    blockBlob.UploadFromByteArray(ms.ToArray(), 0, (int)file.Length);
                }
                catch (System.ObjectDisposedException e)
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
    }
}
