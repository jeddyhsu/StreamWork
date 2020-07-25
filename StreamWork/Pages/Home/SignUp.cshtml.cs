using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly StorageService storage;
        private readonly EncryptionService encryption;

        public SignUpModel(StorageService storage, EncryptionService encryption)
        {
            this.storage = storage;
            this.encryption = encryption;
        }

        public async Task<JsonResult> OnGetIsAddressAvailable(string emailAddress)
        {
            return new JsonResult(await storage.Get<UserLogin>(SQLQueries.GetUserWithEmailAddress, emailAddress) == null);
        }

        public async Task<JsonResult> OnGetIsUsernameAvailable(string username)
        {
            return new JsonResult(await storage.Get<UserLogin>(SQLQueries.GetUserWithUsername, username) == null);
        }

        public async Task OnPostSignUpStudent()
        {
            string id = Guid.NewGuid().ToString();

            if (Request.Form.ContainsKey("Token"))
            {
                await SignUpOauth(Request, id);
            }
            else
            {
                await storage.Save(id, new UserLogin
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
                    ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                    ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_Banner_svg_SW.svg",
                });
            }

            GetAllSelectedTopics(Request.Form["Topics"].ToString().Split('|')); //we need to save this somewhere
        }

        public async Task OnPostSignUpTutor()
        {
            string id = Guid.NewGuid().ToString();
            await storage.Save(id, new UserLogin
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
                AcceptedTutor = false,
                LastLogin = DateTime.UtcNow,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_Banner_svg_SW.svg",
            });

            await CreateChannel(Request.Form["Username"]);
        }

        public async Task SignUpOauth(HttpRequest request, string id)
        {
            var oauthRequestToken = request.Form["Token"];
            GoogleOauth oauthInfo = storage.Call<GoogleOauth>("https://oauth2.googleapis.com/tokeninfo?id_token=" + oauthRequestToken);

            await storage.Save(id, new UserLogin
            {
                Id = id,
                Name = oauthInfo.Name.Replace(' ','|'),
                EmailAddress = oauthInfo.Email,
                Username = Request.Form["Username"],
                ProfileType = "student",
                College = Request.Form["SchoolName"],
                NotificationSubscribe = "True",
                Expiration = DateTime.UtcNow,
                AcceptedTutor = false,
                LastLogin = DateTime.UtcNow,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_Banner_svg_SW.svg",
            });
        }

        private async Task<bool> CreateChannel(string username)
        {
            string id = Guid.NewGuid().ToString();
            return await storage.Save(id, new UserChannel
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

            Hashtable table = new Hashtable
            {
                { 0, "Humanities" },
                { 1, "Mathematics" },
                { 2, "Science" },
                { 3, "Art" },
                { 4, "Engineering" },
                { 5, "Business" },
                { 6, "Law" },
                { 7, "Other" }
            };

            for (int i = 0; i < subjects.Length; i++)
            {
                if (subjects[i] == "true")
                {
                    selectedSubjects.Add((string)table[i]);
                }
            }

            return selectedSubjects;
        }
    }
}
