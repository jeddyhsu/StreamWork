using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly SessionService session;
        private readonly StorageService storage;
        private readonly EncryptionService encryption;

        public UserLogin GenericUserProfile { get; set; }

        public SignUpModel(SessionService session, StorageService storage, EncryptionService encryption)
        {
            this.session = session;
            this.storage = storage;
            this.encryption = encryption;
        }

        public async Task OnGet()
        {
            GenericUserProfile = await session.GetCurrentUser();
        }

        public async Task<JsonResult> OnGetIsAddressAvailable(string emailAddress)
        {
            return new JsonResult(await storage.Get<UserLogin>(SQLQueries.GetUserWithEmailAddress, emailAddress) == null);
        }

        public async Task<JsonResult> OnGetIsUsernameAvailable(string username)
        {
            return new JsonResult(await storage.Get<UserLogin>(SQLQueries.GetUserWithUsername, username) == null);
        }

        public async Task OnGetSignUpStudent(string emailAddress, bool inCollege, string schoolName,
            bool humanitiesTopic, bool mathTopic, bool scienceTopic, bool artTopic, bool engineeringTopic, bool businessTopic, bool lawTopic, bool otherTopic,
            string firstName, string lastName, string username, string password)
        {
            string id = Guid.NewGuid().ToString();
            await storage.Save(id, new UserLogin
            {
                Id = id,
                Name = firstName + "|" + lastName,
                EmailAddress = emailAddress,
                Username = username,
                Password = encryption.EncryptPassword(password),
                ProfileType = "student",
                College = schoolName,
                NotificationSubscribe = "True",
                Expiration = DateTime.UtcNow,
                AcceptedTutor = false,
                LastLogin = DateTime.UtcNow,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Placeholder_pfp_SW.png",
                ProfileBanner = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/Profile_Banner_Placeholder_SW.png",
            });
        }

        public async Task OnPostSignUpTutor(string emailAddress, bool inCollege, string schoolName) //etc
        {
            // Also need to send email
        }
    }
}
