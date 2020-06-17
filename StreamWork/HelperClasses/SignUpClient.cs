using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.HelperClasses
{
    public class SignUpClient
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();
        private readonly EmailMethods _emailMethods = new EmailMethods();
        private readonly IOptionsSnapshot<StorageConfig> _storageConfig;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _emailAddress;
        private readonly string _payPalAddress;
        private readonly string _username;
        private readonly string _password;
        private readonly string _college;
        private readonly string _role;
        private readonly IFormFile _transcript;
        private readonly IFormFile _resume;

        public SignUpClient([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request)
        {
            _storageConfig = storageConfig;
            _firstName = request.Form["FirstName"];
            _lastName = request.Form["LastName"];
            _emailAddress = request.Form["EmailAddress"];
            if(request.Form.ContainsKey("PayPalAddress"))
                _payPalAddress = request.Form["PayPalAddress"];
            _username = request.Form["Username"];
            _password = request.Form["Password"];
            if (request.Form.ContainsKey("College"))
                _college = request.Form["College"];
            _role = request.Form["Role"];
            if(request.Form.Files.Count > 1)
            {
                _transcript = request.Form.Files[0];
                _resume = request.Form.Files[1];
            }
        }

        public async Task<string> HandleSignUp()
        {
            var isThereDuplicate = await CheckForDuplicateUsernameOrEmailOrPayPalEmail();
            if (isThereDuplicate != null) return isThereDuplicate;
            if (await SignUpUser()) return JsonResponse.Success.ToString();

            return JsonResponse.Failed.ToString();
        }

        private async Task<string> CheckForDuplicateUsernameOrEmailOrPayPalEmail()
        {
            var checkEmail = await _homeMethods.GetUserProfiles(_storageConfig, SQLQueries.GetUserWithEmailAddress, _emailAddress);
            if (checkEmail.Count != 0) return JsonResponse.EmailExists.ToString();

            if (_role.Equals("tutor"))
            {
                var checkPayPalEmailUsingRegularEmail = await _homeMethods.GetUserProfiles(_storageConfig, SQLQueries.GetUserWithEmailAddress, _payPalAddress); //payPal email can't be someone elses regular email
                var checkPayPalEmail = await _homeMethods.GetUserProfiles(_storageConfig, SQLQueries.GetUserWithPayPalAddress, _payPalAddress);
                if (checkPayPalEmailUsingRegularEmail.Count != 0 || checkPayPalEmail.Count != 0) return JsonResponse.PayPalEmailExists.ToString();
            }

            var checkUsername = await _homeMethods.GetUserProfiles(_storageConfig, SQLQueries.GetUserWithUsername, _username);
            if (checkUsername.Count != 0) return JsonResponse.UsernameExists.ToString();

            return null;
        }

        private async Task<bool> SignUpUser()
        {
            try
            {
                UserLogin userProfile = new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = _firstName + "|" + _lastName,
                    EmailAddress = _emailAddress,
                    Username = _username,
                    Password = _encryptionMethods.EncryptPassword(_password),
                    ProfileType = _role,
                    AcceptedTutor = false,
                    College = _college,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                    ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Profile_Banner_Placeholder_SW.png",
                    Balance = (decimal)0f,
                    Expiration = DateTime.UtcNow,
                    TrialAccepted = false,
                    PayPalAddress = _payPalAddress,
                    NotificationSubscribe = DatabaseValues.True.ToString()
                };

                if (userProfile.ProfileType == "student") await _emailMethods.SendOutEmailToStreamWorkTeam(userProfile);
                await DataStore.SaveAsync(_homeMethods._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);
                if (userProfile.ProfileType == "tutor") await CreateChannel();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SignUpLoginClient-CreateChannel: " + e.Message);
                return false;
            }
        }

        private async Task<bool> CreateChannel()
        {
            try
            {
                //Create User Channel For Tutor
                UserChannel userChannel = new UserChannel
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = _username,
                    ChannelKey = _homeMethods._defaultStreamHosterChannelKey,
                    StreamSubject = null,
                    StreamThumbnail = null,
                    StreamTitle = null,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png",
                };
                await DataStore.SaveAsync(_homeMethods._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                await SendTutorInfoToStreamWorkTeam();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SignUpLoginClient-CreateChannel: " + e.Message);
                return false;
            }
        }

        private async Task SendTutorInfoToStreamWorkTeam()
        {
            try{
                List<Attachment> attachments = new List<Attachment>();
                attachments.Add(new Attachment(_transcript.OpenReadStream(), _transcript.FileName));
                attachments.Add(new Attachment(_resume.OpenReadStream(), _resume.FileName));
                await _emailMethods.SendOutEmailToStreamWorkTeam(_firstName, _lastName, _emailAddress, attachments);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SignUpLoginClient-CreateChannel: " + e.Message);
            }
        }
    }
}
