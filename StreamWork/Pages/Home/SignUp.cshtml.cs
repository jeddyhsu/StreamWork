using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Oauth;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class SignUpModel : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storage;
        private readonly EncryptionService encryption;
        private readonly TopicService topics;

        public SignUpModel(StorageService storage, EncryptionService encryption, CookieService cookie, TopicService topics)
        {
            this.storage = storage;
            this.encryption = encryption;
            cookieService = cookie;
            this.topics = topics;
        }

        public async Task<JsonResult> OnGetIsAddressAvailable(string emailAddress)
        {
            return new JsonResult(await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, emailAddress) == null);
        }

        public JsonResult OnGetIsAddressValid(string emailAddress)
        {
            try
            {
                return new JsonResult(new MailAddress(emailAddress).Address == emailAddress);
            }
            catch
            {
                return new JsonResult(false);
            }
        }

        public async Task<JsonResult> OnGetIsUsernameAvailable(string username)
        {
            return new JsonResult(await storage.Get<Profile>(SQLQueries.GetUserWithUsername, username) == null);
        }

        public async Task OnPostSignUpStudent()
        {
            if (!await VerifyRequest(Request))
            {
                // JavaScript checks seeem to have been ignored!
                // Potential attack detected. Aborting.
                return;
            }

            string id = Guid.NewGuid().ToString();

            if (Request.Form.ContainsKey("Token"))
            {
                await SignUpOauth(Request, id, "student");
            }
            else
            {
                Profile user = new Profile
                {
                    Id = id,
                    Name = Request.Form["FirstName"] + "|" + Request.Form["LastName"],
                    EmailAddress = Request.Form["EmailAddress"],
                    Username = Request.Form["Username"],
                    Password = encryption.EncryptPassword(Request.Form["Password"]),
                    ProfileType = "student",
                    College = Request.Form["SchoolName"],
                    NotificationSubscribe = "True",
                    Expiration = DateTime.UtcNow,
                    AcceptedTutor = false,
                    LastLogin = DateTime.UtcNow,
                    //ProfileSince = DateTime.UtcNow,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                    ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_Banner_svg_SW.svg",
                };

                storage.Save(user.Id, user).Wait(); // Can't SignIn until user has been created

                await cookieService.SignIn(Request.Form["Username"], encryption.DecryptPassword(user.Password, Request.Form["Password"]));
            }

            topics.FollowTopics(Request.Form["Username"], GetAllSelectedTopics(Request.Form["Topics"].ToString().Split('|')));
        }

        public async Task OnPostSignUpTutor()
        {
            if (!await VerifyRequest(Request))
            {
                // JavaScript checks seeem to have been ignored!
                // Potential attack detected. Aborting.
                return;
            }

            string id = Guid.NewGuid().ToString();
            if (Request.Form.ContainsKey("Token"))
            {
                await SignUpOauth(Request, id, "tutor");
            }
            else
            {
                Profile user = new Profile
                {
                    Id = id,
                    Name = Request.Form["FirstName"] + "|" + Request.Form["LastName"],
                    EmailAddress = Request.Form["EmailAddress"],
                    Username = Request.Form["Username"],
                    Password = encryption.EncryptPassword(Request.Form["Password"]),
                    ProfileType = "tutor",
                    College = Request.Form["SchoolName"],
                    NotificationSubscribe = "True",
                    Expiration = DateTime.UtcNow,
                    PayPalAddress = Request.Form["PayPalAddress"],
                    AcceptedTutor = false,
                    LastLogin = DateTime.UtcNow,
                    //ProfileSince = DateTime.UtcNow,
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                    ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_Banner_svg_SW.svg",
                };

                storage.Save(user.Id, user).Wait(); // Can't SignIn until user has been created

                await cookieService.SignIn(Request.Form["Username"], encryption.DecryptPassword(user.Password, Request.Form["Password"]));
            }

            await CreateChannel(Request.Form["Username"]);
            //need to email transcript and resume

            topics.TutorTopics(Request.Form["Username"], GetAllSelectedTopics(Request.Form["Topics"].ToString().Split('|')));
        }

        // Server-side security checks
        private async Task<bool> VerifyRequest(HttpRequest request)
        {
            string emailAddress = request.Form["EmailAddress"];
            string username = request.Form["Username"];
            Regex nameRegex = new Regex(@"^[^0-9\t\n\/<>?;:""`!@#$%^&*()\[\]{}_+=|\\]+$");
            try
            {
                return
                    nameRegex.IsMatch(request.Form["FirstName"]) &&
                    nameRegex.IsMatch(request.Form["LastName"]) &&
                    new Regex(@"^[A-Za-z0-9_-]+$").IsMatch(username) &&
                    new MailAddress(emailAddress).Address == emailAddress &&
                    await storage.Get<Profile>(SQLQueries.GetUserWithUsername, username) == null &&
                    await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, emailAddress) == null;
            }
            catch
            {
                return false;
            }
        }

        public async Task SignUpOauth(HttpRequest request, string id, string type)
        {
            var oauthRequestToken = request.Form["Token"];
            GoogleOauth oauthInfo = storage.CallJSON<GoogleOauth>("https://oauth2.googleapis.com/tokeninfo?id_token=" + oauthRequestToken);
            var password = encryption.EncryptPassword("!!0_STREAMWORK_!!0");

            await storage.Save(id, new Profile
            {
                Id = id,
                Name = oauthInfo.Name.Contains(' ') ? oauthInfo.Name.Replace(' ', '|') : oauthInfo.Name + "|",
                EmailAddress = oauthInfo.Email,
                Username = Request.Form["Username"],
                Password = password,
                ProfileType = type,
                College = Request.Form["SchoolName"],
                NotificationSubscribe = "True",
                Expiration = DateTime.UtcNow,
                AcceptedTutor = false,
                LastLogin = DateTime.UtcNow,
                //ProfileSince = DateTime.UtcNow,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_Banner_svg_SW.svg",
            });

            await cookieService.SignIn(Request.Form["Username"], encryption.DecryptPassword(password, "!!0_STREAMWORK_!!0"));
        }

        public async Task<IActionResult> OnPostCheckIfOauthUserExists(string email)
        {
            var userProfile = await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, email);
            if (userProfile != null)
            {
                await cookieService.SignIn(userProfile.Username, userProfile.Password);
                return new JsonResult(userProfile.ProfileType);
            }

            return new JsonResult(null);
        }

        private async Task<bool> CreateChannel(string username)
        {
            string id = Guid.NewGuid().ToString();
            return await storage.Save(id, new Channel
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                ChannelKey = "Ec9jbSsc880_5",
                StreamSubject = null,
                StreamThumbnail = null,
                StreamTitle = null,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
            });
        }

        private List<string> GetAllSelectedTopics(string[] subjects)
        {
            List<string> selectedSubjects = new List<string>();

            string[] allSubjects = { "Humanities", "Mathematics", "Science", "Art", "Engineering", "Business", "Law", "Other" };

            for (int i = 0; i < subjects.Length; i++)
            {
                if (subjects[i] == "true")
                {
                    selectedSubjects.Add(allSubjects[i]);
                }
            }

            return selectedSubjects;
        }
    }
}
