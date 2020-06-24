using System;
using System.Collections;
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
        private readonly ScheduleMethods _scheduleMethods = new ScheduleMethods();

        public async Task<string[]> EditProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request, string user)
        {
            IFormFile profilePicture = null;
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
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

            await _scheduleMethods.UpdateTimezoneForScheduleTask(storageConfig, timeZone, userProfile.Username);

            if (profilePicture != null)
            {
                userProfile.ProfilePicture = _blobMethods.SaveImageIntoBlobContainer(profilePicture, userProfile.Id, 240, 320);
                if (userProfile.ProfileType == "tutor")
                    await _tutorMethods.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, userProfile.Username, userProfile.ProfilePicture); //only if tutor
            }
                
            await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            return new string[] { firstName, lastName, occupation, location, timeZone, linkedInUrl, userProfile.ProfilePicture };
        }

        public async Task<string> SaveBanner([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request, string user)
        {
            try
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
                IFormFile profileBanner = request.Form.Files[0];
                var banner = _blobMethods.SaveImageIntoBlobContainer(profileBanner, userProfile.Username + "-" + userProfile.Id + "-profilebanner", 870, 254);
                userProfile.ProfileBanner = banner;
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return banner;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in EditProfileMethods-SaveBanner " + e.Message);
                return null;
            }
        }

        public async Task<bool> SaveUniversity([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user, string abbr, string name)
        {
            try
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);
                userProfile.College = abbr + "|" + name;
                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in EditProfileMethods-SaveUniversity " + e.Message);
                return false;
            }
        }

        //private string GetTimeZoneAbbreviation(string zone)
        //{
        //    Hashtable timeZones = new Hashtable
        //    {
        //        { "GMT -08:00", "PST" },
        //        { "GMT -07:00", "MST" },
        //        { "GMT -06:00", "CST" },
        //        { "GMT -05:00", "EST" },
        //        { "GMT -09:00", "Alaska" },
        //        { "GMT -10:00", "Hawaii" },
        //    };

        //    return (string)timeZones[zone];
        //}
    }
}
