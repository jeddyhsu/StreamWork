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
using StreamWork.Threads;

namespace StreamWork.Controllers
{
    public class TutorController : Controller
    {
        HelperFunctions helperFunctions = new HelperFunctions();

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpGet]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userLogins = await helperFunctions.GetUserLogins(storageConfig, "CurrentUser", user),
                userChannels = await helperFunctions.GetUserChannels(storageConfig, "CurrentUserChannel", user),
                userArchivedVideos = await helperFunctions.GetArchivedStreams(storageConfig, "UserArchivedVideos", user)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamTitle, string streamSubject, string change, string channelKey, string stop)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            var userChannel = await helperFunctions.GetUserChannels(storageConfig, "CurrentUserChannel", user);

            if (channelKey != null)
            {
                if (userChannel[0].ChannelKey == null)
                {
                    try
                    {
                        var channelInfo = DataStore.CallChannelAPI("http://api.dacast.com/v2/channel/+" + channelKey + "?apikey=135034_9d5e445816dfcd2a96ad&_format=JSON");
                        userChannel[0].ChannelKey = channelInfo.Id.ToString();
                        await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                        return Json(new { Message = "Success" });
                    }
                    catch (System.Net.WebException e)
                    {
                        return Json(new { Message = "Failed" });
                    }
                }
            }

            //Saves streamTitle, URl, and subject into sql database
            if (streamTitle != null && streamSubject != null)
            {
                //Need some way of checking that stream is playing so that a streamer cannot start going live on streamWork before obs!!!!!!!!

                ThreadClass handlevideoarchiving = new ThreadClass(storageConfig, userChannel[0], streamTitle, streamSubject);
                handlevideoarchiving.RunThread();
                return Json(new { Message = "Saved" });
            }
            //change stream subject
            if (change != null)
            {
                userChannel[0].StreamSubject = change;
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                return Json(new { Message = "Saved" });
            }

            if (stop != null)
            {
                return Json(new { Message = "Stopped" });
            }

            return Json(new { Message = "Failed" });
        }

        [HttpGet]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", user),
                userChannels = await helperFunctions.GetUserChannels(storageConfig, "CurrentUserChannel", user),
                userArchivedVideos = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { user })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string stop)
        {
            var user = HttpContext.Session.GetString("UserProfile");

            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                var file = Request.Form.Files[i];
                var fileSplit = file.Name.Split(new char[] { '|' });
                var profileCaption = fileSplit[0];
                var profileParagraph = fileSplit[1];
                await helperFunctions.SaveIntoBlobContainer(file, profileCaption, profileParagraph, storageConfig, user);
                return Json(new { Message = "Success" });
            }

            if (Request.Form.Keys.Count > 0)
            {
                var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "CurrentUser", new List<string> { user });

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
            return Json(new { Message = "" });
        }
    }
}
