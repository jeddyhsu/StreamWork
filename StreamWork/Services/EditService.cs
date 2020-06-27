using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class EditService : StorageService
    {
        public EditService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public async Task<List<string>> EditProfile(HttpRequest request, string user)
        {
            IFormFile profilePicture = null;
            var userProfile = await Get<UserLogin>(SQLQueries.GetUserWithUsername, user);
            var firstName = request.Form["FirstName"];
            var lastName = request.Form["LastName"];
            var occupation = request.Form["Occupation"];
            var location = request.Form["Location"];
            var timeZone = request.Form["Timezone"];
            var linkedInUrl = request.Form["LinkedInUrl"];

            if (request.Form.Files.Count > 0)
                profilePicture = request.Form.Files[0];

            userProfile.Name = firstName + "|" + lastName;
            userProfile.ProfileCaption = occupation;
            userProfile.Location = location;
            userProfile.TimeZone = timeZone;
            userProfile.LinkedInUrl = linkedInUrl;

            //await _scheduleMethods.UpdateTimezoneForScheduleTask(storageConfig, timeZone, userProfile.Username);

            if (profilePicture != null)
            {
                userProfile.ProfilePicture = BlobMethods.SaveImageIntoBlobContainer(profilePicture, userProfile.Id, 240, 320);
                //if (userProfile.ProfileType == "tutor")
                //await _tutorMethods.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, userProfile.Username, userProfile.ProfilePicture); //only if tutor
            }

            await Save<UserLogin>(userProfile.Id, userProfile);

            return new List<string> { firstName, lastName, occupation, location, timeZone, linkedInUrl, userProfile.ProfilePicture };
        }

        public async Task<string> SaveBanner(HttpRequest request, string user)
        {
            try
            {
                var userProfile = await Get<UserLogin>(SQLQueries.GetUserWithUsername, user);
                IFormFile profileBanner = request.Form.Files[0];
                var banner = BlobMethods.SaveImageIntoBlobContainer(profileBanner, userProfile.Username + "-" + userProfile.Id + "-profilebanner", 720, 242);
                userProfile.ProfileBanner = banner;
                await Save<UserLogin>(userProfile.Id, userProfile);
                return banner;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in EditProfileMethods-SaveBanner " + e.Message);
                return null;
            }
        }

        public async Task<bool> SaveUniversity(string user, string abbr, string name)
        {
            try
            {
                var userProfile = await Get<UserLogin>(SQLQueries.GetUserWithUsername, user);
                userProfile.College = abbr + "|" + name;
                await Save<UserLogin>(userProfile.Id, userProfile);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in EditProfileMethods-SaveUniversity " + e.Message);
                return false;
            }
        }
    }
}
