using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using StreamWork.ViewModels;
using StreamWork.DataModels;

// THE FOLLOWING WILL BE DELETED AFTER TESTING
using System.IO;
using System.Net;
using System.Text;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public IActionResult Index()
        {
            if (Request.Host.ToString() == "streamwork.live")
            {
                return Redirect("https://www.streamwork.live");
            }
            return View();
        }

        public async Task<IActionResult> Math([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Mathematics"));
        }

        public async Task<IActionResult> Science([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Science"));
        }

        public async Task<IActionResult> Engineering([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Engineering"));
        }

        public async Task<IActionResult> Business([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Business"));
        }

        public async Task<IActionResult> Law([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Law"));
        }

        public async Task<IActionResult> DesignArt([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Art"));
        }

        public async Task<IActionResult> Humanities([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Humanities"));
        }

        public async Task<IActionResult> Other([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View(await PopulateSubjectPage(storageConfig, "Other"));
        }

        public IActionResult BecomeTutor()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult HowToStream()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileView(string Tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            ProfileTutorViewModel profile = new ProfileTutorViewModel
            {
                userArchivedVideos = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, "UserArchivedVideos", new List<string> { Tutor }),
                userLogins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { Tutor })
            };
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Donate(string Tutor, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            ProfileTutorViewModel profile = new ProfileTutorViewModel {
                userLogins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { Tutor })
            };

            Donation donation = new Donation {
                Id = Guid.NewGuid().ToString(),
                Student = HttpContext.Session.GetString("UserProfile"),
                Tutor = profile.userLogins[0].Username,
                TimeSent = DateTime.Now.ToString(),
            };
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", donation.Id } }, donation);

            return View(profile);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<ProfileTutorViewModel> PopulateSubjectPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var streams = await GetStreams(storageConfig, subject);
            var tutors = await GetPopularStreamTutors(storageConfig);
            ProfileTutorViewModel model = new ProfileTutorViewModel
            {
                userChannels = streams,
                userLogins = tutors
            };
            return model;
        }

        private async Task<List<UserLogin>> GetPopularStreamTutors([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            List<UserLogin> list = new List<UserLogin>();
            var getCurrentUsers = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "AllSignedUpUsers", null);
            foreach (UserLogin user in getCurrentUsers)
            {
                if (user.ProfileType.Equals("tutor"))
                {
                    list.Add(user);
                }
            }
            return list;
        }

        private async Task<List<UserChannel>> GetStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject)
        {
            var getAllStreams = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, "AllStreamKeys", new List<string> { subject });
            List<UserChannel> list = getAllStreams;
            return list;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig,
                                                string nameFirst, string nameLast, string email, string phone, string username, string password, string passwordConfirm, string channelId, string role)
        {
            string confirmation = "";
            UserLogin signUpProflie = new UserLogin
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameFirst + "|" + nameLast,
                EmailAddress = email,
                Username = username,
                Password = password,
                ProfileType = role,
                ProfilePicture = "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/default-profile.png"
            };

            UserChannel key = new UserChannel
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                ChannelKey = null,
                SubjectStreaming = null,
                StreamThumbnail = null,
                StreamTitle = null,
                VideoURL = null
            };
            var checkCurrentUsers = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { username });
            int numberOfUsers = 0;
            foreach (var user in checkCurrentUsers)
            {
                numberOfUsers++;
            }
            if (numberOfUsers != 0)
                confirmation = "Username already exsists";
            else if (password != passwordConfirm)
                confirmation = "Wrong Password";
            else
            {
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProflie.Id } }, signUpProflie);
                await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", key.Id } }, key);
                confirmation = "Success";
            }
            return Json(new { Message = confirmation });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string password)
        {
            string confirmation = "";
            if (storageConfig != null)
            {
                int user = 0;
                var checkforUser = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "AllSignedUpUsersWithPassword", new List<string> { username, password });
                foreach (var u in checkforUser)
                {
                    if (u.Password == password && u.Username == username)
                    {
                        user++;
                        if (u.ProfileType == "tutor")
                        {
                            user++;
                        }
                    }
                }
                if (user == 1)
                {
                    confirmation = "Welcome";
                    HttpContext.Session.SetString("UserProfile", username);
                }
                if (user == 2)
                {
                    confirmation = "Welcome, StreamTutor";
                    HttpContext.Session.SetString("UserProfile", username);
                }
            }
            else
            {
                confirmation = "Wrong Password or Username ";
            }
            return Json(new { Message = confirmation });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //
        //
        // EVERYTHING BEYOND THIS POINT WILL BE REMOVED AFTER TESTING
        //
        //

        private IOptionsSnapshot<StorageConfig> storageConfig;

        private class IPNContext {
            public HttpRequest IPNRequest { get; set; }

            public string RequestBody { get; set; }

            public string Verification { get; set; } = String.Empty;
        }

        public IActionResult TestDonate ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return Receive(storageConfig);
        }

        // Should run when an IPN is recieved
        [HttpPost]
        public IActionResult Receive (IOptionsSnapshot<StorageConfig> _storageConfig) {
            storageConfig = _storageConfig;

            IPNContext ipnContext = new IPNContext {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII)) {
                ipnContext.RequestBody = reader.ReadToEnd();
            }

            //Store the IPN received from PayPal
            Donation donation = LogRequest(ipnContext);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(ipnContext, donation));

            //Reply back a 200 code
            return Ok();
        }

        private async Task VerifyTask (IPNContext ipnContext, Donation donation) {
            try {
                var verificationRequest = WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";

                //Add cmd=_notify-validate to the payload
                string strRequest = "cmd=_notify-validate&" + ipnContext.RequestBody;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII)) {
                    writer.Write(strRequest);
                }

                //Send the request to PayPal and get the response
                using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream())) {
                    ipnContext.Verification = reader.ReadToEnd();
                }
            }
            catch (Exception exception) {
                //Capture exception for manual investigation
                donation.Error += "...Failed to verify IPN task: " + exception.Message;
            }

            await ProcessVerificationResponse(ipnContext, donation);
        }

        // Should find the table entry created when payment was made, set verified to FALSE, and return it, and optionally discard incomplete donations.
        private Donation LogRequest (IPNContext ipnContext) {
            // Persist the request values into a database or temporary data store

            return new Donation {
                Id = Guid.NewGuid().ToString(), // Generated earlier
                Student = "student123", // Earlier
                Tutor = "streamtutor123", // Earlier
                Val = "20.00",
                TimeSent = DateTime.Now.ToString(), // Earlier
                Verified = "FALSE",
                Error = ""
            };
        }

        // Should update table entry on payment validity
        private async Task ProcessVerificationResponse (IPNContext ipnContext, Donation donation) {
            if (ipnContext.Verification.Equals("VERIFIED")) {
                if (donation.Error.Equals("")) {
                    donation.Verified = "TRUE";
                }

                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment
            }
            else if (ipnContext.Verification.Equals("INVALID")) {
                donation.Error += "...IPN could not be verified because it was found to be INVALID";
            }
            else {
                donation.Error += "...IPN could not be verified for unknown reasons (neither VALID nor INVALID)";
            }

            donation.Error += "...DEBUG NOTES...IPNRequest.Body = " + ipnContext.IPNRequest.Body + "...RequestBody = " + ipnContext.RequestBody + "...Verification = " + ipnContext.Verification;

            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", donation.Id } }, donation);
        }
    }
}
