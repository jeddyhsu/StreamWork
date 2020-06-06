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
    public class EditProfileMethods
    {
        readonly HomeMethods _homeHelperFunctions = new HomeMethods();
        readonly TutorMethods _tutorHelperFunctions = new TutorMethods();

        public async Task<string[]> EditProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request, UserLogin userProfile)
        {
            IFormFile profilePicture = null; 
            var profileCaption = request.Form["ProfileCaption"];
            var profileParagraph = request.Form["ProfileParagraph"];
            if(request.Form.Files.Count > 0)
                profilePicture = request.Form.Files[0];

            userProfile.ProfileCaption = profileCaption;
            userProfile.ProfileParagraph = profileParagraph;
            if (profilePicture != null)
            {
                userProfile.ProfilePicture = _homeHelperFunctions.SaveIntoBlobContainer(profilePicture, userProfile.Id, 240, 320);
                if (userProfile.ProfileType == "tutor")
                    await _tutorHelperFunctions.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, userProfile.Username, userProfile.ProfilePicture); //only if tutor
            }
                
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            return new string[] { profileCaption, profileParagraph, userProfile.ProfilePicture };
        }
    }
}
