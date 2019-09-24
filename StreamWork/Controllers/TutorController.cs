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
using StreamWork.DaCastAPI;
using System.Collections;

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
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string change, string channelKey)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            var userChannel = await helperFunctions.GetUserChannels(storageConfig, "CurrentUserChannel", user);

            if (channelKey != null)
            {
                if (userChannel[0].ChannelKey == null)
                {
                    try
                    {
                        var channelInfo = DataStore.CallAPI<ChannelAPI>("http://api.dacast.com/v2/channel/+" + channelKey + "?apikey=135034_9d5e445816dfcd2a96ad&_format=JSON");
                        userChannel[0].ChannelKey = channelInfo.Id.ToString();
                        await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                        return Json(new { Message = "Success" });
                    }
                    catch (System.Net.WebException e)
                    {
                        e.ToString(); // Literally just did this to get rid of the warning
                        return Json(new { Message = "Failed" });
                    }
                }
            }

            //Saves streamTitle, URl, and subject into sql database
            if (Request.Form.Files.Count != 0)
            {
                var streamInfo = Request.Form.Files[0].Name.Split(new char[] { '|' });
                var streamTitle = streamInfo[0];
                var streamSubject = streamInfo[1];
                var streamThumbnail = await helperFunctions.SaveIntoBlobContainer(Request.Form.Files[0], storageConfig, user, userChannel[0].Id);
                ThreadClass handlevideoarchiving = new ThreadClass(storageConfig, userChannel[0], streamTitle, streamSubject, streamThumbnail);
                handlevideoarchiving.RunThread();
                return Json(new { Message = "Saved" });
            }

            //Saves if there is no thumbnail uploaded
            if (Request.Form.Count != 0)
            {
                var streamInfo = new string[2];
                foreach (var key in Request.Form.Keys)
                {
                     streamInfo = key.Split(new char[] { '|' });
                }
                var streamTitle = streamInfo[0];
                var streamSubject = streamInfo[1];
                ThreadClass handlevideoarchiving = new ThreadClass(storageConfig, userChannel[0], streamTitle, streamSubject, GetCorrespondingDefaultThumbnail(streamSubject));
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
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, "CurrentUser", user);

            //Handles if there is a profile picture with the caption or about paragraph
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var fileSplit = file.Name.Split(new char[] { '|' });
                var profileCaption = fileSplit[0];
                var profileParagraph = fileSplit[1];
                var profilePicture = await helperFunctions.SaveIntoBlobContainer(file, storageConfig, user, userProfile.Id);
                userProfile.ProfileCaption = profileCaption != "NA" ? profileCaption : null;
                userProfile.ProfilePicture = profilePicture;
                userProfile.ProfileParagraph = profileParagraph != "NA" ? profileParagraph : null;
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return Json(new { Message = "Success" });
            }

            //Handles if there is not a profile picture with the caption or about paragraph
            if (Request.Form.Keys.Count > 0)
            {
                var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "CurrentUser", new List<string> { user });
                var profileCaption = "";
                var profileParagraph = "";
                foreach (string s in Request.Form.Keys)
                {
                    var array = s.Split(new char[] { '|' });
                    profileCaption = array[0];
                    profileParagraph = array[1];
                    break;
                }

                getUserInfo[0].ProfileCaption = profileCaption != "NA" ? profileCaption : null;
                getUserInfo[0].ProfileParagraph = profileParagraph != "NA" ? profileParagraph : null;

                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", getUserInfo[0].Id } }, getUserInfo[0]);
                return Json(new { Message = "Success" });
            }
            return Json(new { Message = "" });
        }

        //Uses a hashtable to add default thumbnails based on subject
        private string GetCorrespondingDefaultThumbnail(string subject)
        {
            string defaultURL = "";

            Hashtable defaultPic = new Hashtable
            {
                { "Mathematics", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/MathDefault.png" },
                { "Humanities", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/HumanitiesDefault.png" },
                { "Science", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/ScienceDefault.png" },
                { "Business", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/BusinessDefault.png" },
                { "Engineering", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/EngineeringDefault.png" },
                { "Law", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/LawDefault.png" },
                { "Art", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/ArtDefault.png" },
                { "Other", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/OtherDefualt.png" }
            };

            ICollection key = defaultPic.Keys;

            foreach(string pic in key)
            {
                if (pic == subject)
                {
                    defaultURL = ((string)defaultPic[pic]);
                }
            }
            return defaultURL;
        }
    }
}
