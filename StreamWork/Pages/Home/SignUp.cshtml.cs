using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly StorageService storageService;
        private readonly EncryptionService encryptionService;
        private readonly TopicService topicService;
        private readonly EmailService emailService;
        private readonly FollowService followService;

        public SignUpModel(StorageService storage, EncryptionService encryption, CookieService cookie, TopicService topics, EmailService email, FollowService follow)
        {
            storageService = storage;
            encryptionService = encryption;
            cookieService = cookie;
            topicService = topics;
            emailService = email;
            followService = follow;
        }

        public async Task<JsonResult> OnGetIsAddressAvailable(string emailAddress)
        {
            return new JsonResult(await storageService.Get<Profile>(SQLQueries.GetUserWithEmailAddress, emailAddress) == null);
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
            return new JsonResult(await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, username) == null);
        }

        public async Task OnGetSendVerificationEmail(string emailAddress)
        {
            EmailVerification emailVerification = await storageService.Get<EmailVerification>(SQLQueries.GetEmailVerificationWithAddress, emailAddress);
            if (emailVerification == null)
            {
                string verificationCode = new Random().Next(10000000, 99999999).ToString();

                string id = Guid.NewGuid().ToString();
                await storageService.Save(id, new EmailVerification
                {
                    Id = id,
                    EmailAddress = emailAddress,
                    VerificationCode = verificationCode
                });

                await emailService.SendEmailVerification(emailAddress, verificationCode);
            }
            else
            {
                await emailService.SendEmailVerification(emailAddress, emailVerification.VerificationCode);
            }
        }

        public async Task<JsonResult> OnGetCheckVerificationCode(string emailAddress, string verificationCode)
        {
            return new JsonResult(await storageService.Get<EmailVerification>(SQLQueries.GetEmailVerificationWithAddressAndCode, emailAddress, verificationCode) != null);
        }

        public async Task OnPostSignUpStudent()
        {
          
            string id = Guid.NewGuid().ToString();

            if (Request.Form.ContainsKey("Token"))
            {
                await SignUpOauth(Request, id, "student");
            }
            else
            {
                if (!await VerifyRequest(Request)) //TOM this wont work with google ouath. The email wont be populated becasue it gets it from an api call LOOK at SignUpOauth
                {
                    // JavaScript checks seeem to have been ignored!
                    // Potential attack detected. Aborting.
                    return;
                }

                Profile user = new Profile
                {
                    Id = id,
                    Name = Request.Form["FirstName"] + "|" + Request.Form["LastName"],
                    EmailAddress = Request.Form["EmailAddress"],
                    Username = Request.Form["Username"],
                    Password = encryptionService.EncryptPassword(Request.Form["Password"]),
                    ProfileType = "student",
                    College = Request.Form["SchoolName"],
                    NotificationSubscribe = "True",
                    Expiration = DateTime.UtcNow,
                    AcceptedTutor = false,
                    LastLogin = DateTime.UtcNow,
                    ProfileColor = MiscHelperMethods.GetRandomColor(),
                    ProfileSince = DateTime.UtcNow,
                    ProfilePicture = MiscHelperMethods.defaultProfilePicture,
                    ProfileBanner = MiscHelperMethods.defaultBanner,
                };

                await storageService.Save(user.Id, user);
                await cookieService.SignIn(Request.Form["Username"], encryptionService.DecryptPassword(user.Password, Request.Form["Password"]));
            }

            topicService.FollowTopics(Request.Form["Username"], GetAllSelectedTopics(Request.Form["Topics"].ToString().Split('|')));
            await emailService.SendTemplateToStreamwork("studentSignUp", await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, Request.Form["Username"]), Request.Form.Files);
        }

        public async Task OnPostSignUpTutor()
        {
            string id = Guid.NewGuid().ToString();
            if (Request.Form.ContainsKey("Token"))
            {
                await SignUpOauth(Request, id, "tutor");
            }
            else
            {
                if (!await VerifyRequest(Request)) //TOM this wont work with google ouath. The email wont be populated becasue it gets it from an api call LOOK at SignUpOauth
                {
                    // JavaScript checks seeem to have been ignored!
                    // Potential attack detected. Aborting.
                    return;
                }

                Profile user = new Profile
                {
                    Id = id,
                    Name = Request.Form["FirstName"] + "|" + Request.Form["LastName"],
                    EmailAddress = Request.Form["EmailAddress"],
                    Username = Request.Form["Username"],
                    Password = encryptionService.EncryptPassword(Request.Form["Password"]),
                    ProfileType = "tutor",
                    College = Request.Form["SchoolName"],
                    NotificationSubscribe = "True",
                    Expiration = DateTime.UtcNow,
                    PayPalAddress = Request.Form["PayPalAddress"],
                    AcceptedTutor = false,
                    LastLogin = DateTime.UtcNow,
                    ProfileColor = MiscHelperMethods.GetRandomColor(),
                    ProfileSince = DateTime.UtcNow,
                    ProfilePicture = MiscHelperMethods.defaultProfilePicture,
                    ProfileBanner = MiscHelperMethods.defaultBanner,
                };

                await storageService.Save(user.Id, user);
                await AddUs5AsFollowers(user.Id);
                await cookieService.SignIn(Request.Form["Username"], encryptionService.DecryptPassword(user.Password, Request.Form["Password"]));
            }

            await CreateChannel(Request.Form["Username"]);
            topicService.TutorTopics(Request.Form["Username"], GetAllSelectedTopics(Request.Form["Topics"].ToString().Split('|')));

            await emailService.SendTemplateToStreamwork("tutorSignUp", await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, Request.Form["Username"]), Request.Form.Files);
        }

        // Server-side security checks
        private async Task<bool> VerifyRequest(HttpRequest request) //TOM this wont work with google sign in
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
                    await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, username) == null &&
                    await storageService.Get<Profile>(SQLQueries.GetUserWithEmailAddress, emailAddress) == null;
            }
            catch
            {
                return false;
            }
        }

        public async Task SignUpOauth(HttpRequest request, string id, string type)
        {
            var oauthRequestToken = request.Form["Token"];
            GoogleOauth oauthInfo = await storageService.CallJSON<GoogleOauth>("https://oauth2.googleapis.com/tokeninfo?id_token=" + oauthRequestToken); //GETS EMAIL FOR GOOGLE OAUTH
            var password = encryptionService.EncryptPassword("!!0_STREAMWORK_!!0");

            await storageService.Save(id, new Profile
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
                ProfileColor = MiscHelperMethods.GetRandomColor(),
                ProfileSince = DateTime.UtcNow,
                ProfilePicture = MiscHelperMethods.defaultProfilePicture,
                ProfileBanner = MiscHelperMethods.defaultBanner,
            });

            if(type == "tutor")
            {
                await AddUs5AsFollowers(id);
            }
                
            await cookieService.SignIn(Request.Form["Username"], encryptionService.DecryptPassword(password, "!!0_STREAMWORK_!!0"));
        }

        public async Task<IActionResult> OnPostCheckIfOauthUserExists(string email, string route)
        {
            var userProfile = await storageService.Get<Profile>(SQLQueries.GetUserWithEmailAddress, email);
            if (userProfile != null)
            {
                var signInProfile = await cookieService.SignIn(userProfile.Username, userProfile.Password);
                if (signInProfile != null)
                    if (route.Contains("Chat") || route.Contains("chat") || route == "/")
                        return new JsonResult(new { Message = "Route", Route = route });
                    else
                    {
                        var r = route.Split("/");
                        if(r.Length > 3)
                        {
                            return cookieService.Route(signInProfile, r[3], encryptionService); //if there is a page there is a page that they were intially going to and got blovked by the signin
                        }

                        return cookieService.Route(signInProfile, "SW", encryptionService);
                    }
            }

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        private async Task<bool> CreateChannel(string username)
        {
            string id = Guid.NewGuid().ToString();
            return await storageService.Save(id, new Channel
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                ChannelKey = "Ec9jbSsc880_5",
                StreamSubject = null,
                StreamThumbnail = null,
                StreamTitle = null,
                ProfilePicture = MiscHelperMethods.defaultProfilePicture,
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

        private async Task AddUs5AsFollowers(string userId)
        {
            await followService.AddFollower("bb709202-2179-4924-ad0d-80f34dd729d5", userId); //Jared
            await followService.AddFollower("168e433d-1352-4a48-b22b-fa6d889812fe", userId); //Rithvik
            await followService.AddFollower("4e55b81f-ab0a-4bc5-bda8-b0313e12ac9f", userId); //Tom
            await followService.AddFollower("f8780448-8085-41ab-9f41-11ea50264522", userId); //Ken
            await followService.AddFollower("ce4e0497-1d37-4145-9021-617a65812ca8", userId); //Junshu
        }
    }
}
