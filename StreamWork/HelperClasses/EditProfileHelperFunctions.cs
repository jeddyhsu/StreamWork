using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;

namespace StreamWork.HelperClasses
{
    public class EditProfileHelperFunctions
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        readonly TutorHelperFunctions _tutorHelperFunctions = new TutorHelperFunctions();

        public async Task<bool> EditProfileWithProfilePicture(HttpRequest Request, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin userProfile, string user)
        {
            try
            {
                var file = Request.Form.Files[0];
                var fileSplit = file.Name.Split(new char[] { '|' });
                var profileCaption = fileSplit[0];
                var profileParagraph = fileSplit[1];
                var profilePicture =  _homeHelperFunctions.SaveIntoBlobContainer(_homeHelperFunctions.ResizeImage(file, 240, 320), file, userProfile.Id);

                userProfile.ProfileCaption = profileCaption != "NA" ? profileCaption : null;
                userProfile.ProfilePicture = profilePicture;
                userProfile.ProfileParagraph = profileParagraph != "NA" ? profileParagraph : null;

                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

                if (userProfile.ProfileType == "tutor")
                    await _tutorHelperFunctions.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, user, profilePicture); //only if tutor
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in EditProfileWithProfilePicture: " + ex.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> EditProfileWithNoProfilePicture(HttpRequest Request, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            try
            {
                var getUserInfo = await DataStore.GetListAsync<UserLogin>(_homeHelperFunctions._connectionString, storageConfig.Value, "CurrentUser", new List<string> { user });
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

                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", getUserInfo[0].Id } }, getUserInfo[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in EditProfileWithNoProfilePicture: " + ex.Message);
                return false;
            }

            return true;
        }
    }
}
