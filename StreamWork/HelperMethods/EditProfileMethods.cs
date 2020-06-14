using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;

namespace StreamWork.HelperMethods
{
    public class EditProfileMethods
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly TutorMethods _tutorMethods = new TutorMethods();
        private readonly BlobMethods _blobMethods = new BlobMethods();

        public async Task<string[]> EditProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request, string user)
        {
            IFormFile profilePicture = null;
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
            var firstName = request.Form["FirstName"];
            var lastName = request.Form["LastName"];
            var occupation = request.Form["Occupation"];
            var location = request.Form["Location"];
            var timezone = request.Form["Timezone"];
            var linkedInUrl = request.Form["LinkedInUrl"];

            if (request.Form.Files.Count > 0)
                profilePicture = request.Form.Files[0];

            userProfile.Name = firstName + "|" + lastName;
            userProfile.ProfileCaption = occupation;
            userProfile.Location = location;
            userProfile.TimeZone = timezone;
            userProfile.LinkedInUrl = linkedInUrl;

            if (profilePicture != null)
            {
                userProfile.ProfilePicture = _blobMethods.SaveImageIntoBlobContainer(profilePicture, userProfile.Id, 240, 320);
                if (userProfile.ProfileType == "tutor")
                    await _tutorMethods.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, userProfile.Username, userProfile.ProfilePicture); //only if tutor
            }
                
            await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            return new string[] { firstName, lastName, occupation, location, timezone, linkedInUrl, userProfile.ProfilePicture };
        }
    }
}
