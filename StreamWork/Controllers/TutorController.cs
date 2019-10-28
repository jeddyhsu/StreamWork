using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using StreamWork.ViewModels;
using StreamWork.DataModels;
using StreamWork.Threads;
using StreamWork.DaCastAPI;
using StreamWork.HelperClasses;
using System.Web;
using System;

namespace StreamWork.Controllers
{
    public class TutorController : Controller
    {
        HelperFunctions helperFunctions = new HelperFunctions();
        TutorHelperFunctions tutorHelperFunctions = new TutorHelperFunctions();

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpGet]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user),
                userArchivedVideos = await helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, user),
                ChatSecretKey = await GetChatSecretKey(storageConfig, user)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string change, string channelKey)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            var userChannel = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            var userLogin = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user);

            if (channelKey != null)
            {
                if (userChannel[0].ChannelKey == null)
                {
                    try
                    {
                        var channelInfo = DataStore.CallAPI<ChannelAPI>("http://api.dacast.com/v2/channel/+" + channelKey + "?apikey=135034_9d5e445816dfcd2a96ad&_format=JSON");
                        userChannel[0].ChannelKey = channelInfo.Id.ToString();
                        await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                        return Json(new { Message = JsonResponse.Success.ToString()});
                    }
                    catch (System.Net.WebException e)
                    {
                        e.ToString(); // Literally just did this to get rid of the warning
                        return Json(new { Message = JsonResponse.Failed.ToString()});
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
                ThreadClass handleStreams = new ThreadClass(storageConfig, userChannel[0], userLogin[0], streamTitle, streamSubject, streamThumbnail);
                await handleStreams.StartRecordingStream();
                return Json(new { Message = JsonResponse.Success.ToString()});
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
                ThreadClass handleStreams = new ThreadClass(storageConfig, userChannel[0], userLogin[0], streamTitle, streamSubject, tutorHelperFunctions.GetCorrespondingDefaultThumbnail(streamSubject));
                await handleStreams.StartRecordingStream();
                handleStreams.RunThread();
                return Json(new { Message = JsonResponse.Success.ToString()});
            }

            //change stream subject
            if (change != null) 
            {
                userChannel[0].StreamSubject = change;
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
                return Json(new { Message = JsonResponse.Success.ToString()});
            }

            return Json(new { Message = JsonResponse.Failed.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");

            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user),
                userArchivedVideos = await helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, user)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string stop)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);

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
                if(userProfile.ProfileType == "tutor")
                    await tutorHelperFunctions.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, user, profilePicture); //only if tutor
                return Json(new { Message = JsonResponse.Success.ToString() });
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
                return Json(new { Message = JsonResponse.Success.ToString()});
            }
            return Json(new { Message = "" });
        }

        [HttpGet]
        public async Task<IActionResult> TutorSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            ProfileTutorViewModel viewModel = new ProfileTutorViewModel
            {
                userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                userLogins = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user),
                userChannels = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user),
                userArchivedVideos = await helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, user)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TutorSettings([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string name,
                                                                                                                     string profileCaption,
                                                                                                                     string profileParagraph,
                                                                                                                     string currentPassword,
                                                                                                                     string newPassword,
                                                                                                                     string confirmPassword)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            var userProfile = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user);
            userProfile.Name = name;
            userProfile.ProfileCaption = profileCaption;
            userProfile.ProfileParagraph = profileParagraph;
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);


            if(currentPassword != null && newPassword != null && confirmPassword != null)
            {
                var userLogin = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user);
                if(currentPassword == userLogin[0].Password)
                {
                    userLogin[0].Password = newPassword;
                    await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userLogin[0].Id } }, userLogin[0]);
                }
                else
                {
                    return Json(new { Message = JsonResponse.Failed.ToString()});
                }
            }

            return Json(new { Message = JsonResponse.Success.ToString()});
        }

        private async Task<string> GetChatSecretKey([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            var userChannel = await helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            var ids = userChannel[0].ChatId.Split("|");
            var encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(helperFunctions.hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username, "3O08UU-OtQ_rycx3")));
            var finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username + "&sig=" + encodedUrl ;
            return finalString;
        }
    }
}
