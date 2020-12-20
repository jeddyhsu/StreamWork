using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public async Task<List<string>> EditProfile(HttpRequest request, Profile userProfile)
        {
        //    IFormFile profilePicture = null;
        //    var firstName = request.Form["FirstName"];
        //    var lastName = request.Form["LastName"];
        //    var occupation = request.Form["Occupation"];
        //    var location = request.Form["Location"];
        //    var timeZone = request.Form["Timezone"];
        //    var linkedInUrl = request.Form["LinkedInUrl"];
        //    var instagramUrl = request.Form["InstagramUrl"];
        //    var facebookUrl = request.Form["FacebookUrl"];
        //    var twitterUrl = request.Form["TwitterUrl"];
        //    var subscribeToNotifications = request.Form["SubscribeToNotifications"];

        //    if (request.Form.Files.Count > 0)
        //        profilePicture = request.Form.Files[0];

        //    Regex nameRegex = new Regex(@"^[^0-9\t\n\/<>?;:""`!@#$%^&*()\[\]{}_+=|\\]+$");
        //    if (!nameRegex.IsMatch(firstName))
        //    {
        //        firstName = userProfile.Name.Split('|')[0];
        //    }
        //    if (!nameRegex.IsMatch(lastName))
        //    {
        //        lastName = userProfile.Name.Split('|')[1];
        //    }

        //    userProfile.Name = firstName + "|" + lastName;
        //    userProfile.ProfileCaption = occupation;
        //    userProfile.Location = location;
        //    userProfile.TimeZone = timeZone;
        //    userProfile.LinkedInUrl = linkedInUrl;
        //    userProfile.InstagramUrl = instagramUrl;
        //    userProfile.FacebookUrl = facebookUrl;
        //    userProfile.TwitterUrl = twitterUrl;
        //    userProfile.NotificationSubscribe = subscribeToNotifications == "true" ? "True" : "False";

        //    //await _scheduleMethods.UpdateTimezoneForScheduleTask(storageConfig, timeZone, userProfile.Username);

        //    if (profilePicture != null)
        //    {
        //        userProfile.ProfilePicture = await BlobMethods.SaveImageIntoBlobContainer(profilePicture, userProfile.Id, 240, 320);
        //        //if (userProfile.ProfileType == "tutor")
        //        //await _tutorMethods.ChangeAllArchivedStreamAndUserChannelProfilePhotos(storageConfig, userProfile.Username, userProfile.ProfilePicture); //only if tutor
        //    }

        //    await Save(userProfile.Id, userProfile);

        //    return new List<string> { firstName, lastName, occupation, location, timeZone, linkedInUrl, userProfile.ProfilePicture };
            return null;
        }

        public async Task<string> DeleteProfilePicture(Profile userProfile)
        {
            //    try
            //    {
            //        userProfile.ProfilePicture = MiscHelperMethods.defaultProfilePicture;
            //        await Save(userProfile.Id, userProfile);
            //        return MiscHelperMethods.defaultProfilePicture;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Error in EditProfileMethods-SaveBanner " + e.Message);
            //        return null;
            //    }
            return null;
        }

        public async Task<string> SaveBanner(HttpRequest request, Profile userProfile)
        {
        //    try
        //    {
        //        if (request.Form.ContainsKey("ProfileBanner"))
        //        {
        //            userProfile.ProfileBanner = MiscHelperMethods.defaultBanner;
        //            await Save(userProfile.Id, userProfile);
        //            return MiscHelperMethods.defaultBanner;
        //        }

        //        IFormFile profileBanner = request.Form.Files[0];
        //        var banner = await BlobMethods.SaveImageIntoBlobContainer(profileBanner, userProfile.Username + "-" + userProfile.Id + "-profilebanner", 720, 242);
        //        userProfile.ProfileBanner = banner;
        //        await Save(userProfile.Id, userProfile);
        //        return banner;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error in EditProfileMethods-SaveBanner " + e.Message);
        //        return null;
        //    }

            return null;
        }

        public async Task<bool> SaveUniversity(Profile userProfile, string abbr, string name)
        {
            //    try
            //    {
            //        userProfile.College = abbr + "|" + name;
            //        await Save(userProfile.Id, userProfile);
            //        return true;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Error in EditProfileMethods-SaveUniversity " + e.Message);
            //        return false;
            //    }

            return false;
        }
    }
}
