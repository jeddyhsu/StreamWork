using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;

namespace StreamWork.HelperClasses
{
    public class TutorHelperFunctions //For functions involved with tutor code only
    {
        readonly HomeHelperFunctions _helperFunctions = new HomeHelperFunctions();
        //Uses a hashtable to add default thumbnails based on subject
        public string GetCorrespondingDefaultThumbnail(string subject)
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

            foreach (string pic in key)
            {
                if (pic == subject)
                {
                    defaultURL = ((string)defaultPic[pic]);
                }
            }
            return defaultURL;
        }

        public async Task ChangeAllArchivedStreamAndUserChannelProfilePhotos([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user, string profilePicture) //changes all profile photos on streams if user has changed it
        {
            var allArchivedStreams = await _helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, user);
            var userChannel = await _helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            foreach (var stream in allArchivedStreams)
            {
                stream.ProfilePicture = profilePicture;
                await DataStore.SaveAsync(_helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", stream.Id } }, stream);
            }
            userChannel[0].ProfilePicture = profilePicture;
            await DataStore.SaveAsync(_helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
        }

        public async Task<string> GetChatSecretKey([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            var userChannel = await _helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            var ids = userChannel[0].ChatId.Split("|");
            var encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(_helperFunctions.hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username, "3O08UU-OtQ_rycx3")));
            var finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username + "&sig=" + encodedUrl;
            return finalString;
        }
    }
}
