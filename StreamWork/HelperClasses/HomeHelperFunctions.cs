using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.Models;
using StreamWork.ViewModels;

namespace StreamWork.HelperClasses
{
    public class HomeHelperFunctions
    {
        public static bool devEnvironment;
        public readonly string _host = devEnvironment ? "http://localhost:58539" : "http://www.streamwork.live";
        public readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";
        public readonly string _dacastAPIKey = "135034_c2914fb8c32374a13c89";
        public readonly string _streamworkEmailID = "streamworktutor@gmail.com";

        //Gets set of userchannels with the query that you specify
        public async Task<List<UserChannel>> GetUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return channels;
        }

        //Gets a set of archived streams with the query that you specify
        public async Task<List<UserArchivedStreams>> GetArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return archivedStreams;
        }

        //All Archived Streams
        public async Task<List<UserArchivedStreams>> GetArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString());
            return archivedStreams;
        }

        //Gets a set of user logins with the query that you specify
        public async Task<List<UserLogin>> GetUserLogins([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return logins;
        }

        public async Task<UserLogin> GetUserProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (logins.Count > 0) return logins[0];
            return null;
        }

        public async Task UpdateUser([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin user)
        {
            await DataStore.DeleteAsync<UserLogin>(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } });
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } }, user);
        }

        public async Task<ProfileTutorViewModel> PopulateSubjectPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string user)
        {
            ProfileTutorViewModel model = new ProfileTutorViewModel
            {
                UserChannels = await GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreamingWithSpecifiedSubject, subject),
                UserLogins = await GetPopularStreamTutors(storageConfig),
                UserProfile = user != null ? await GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user) : null,
                Subject = subject
            };
            return model;
        }

        public async Task<ProfileTutorViewModel> PopulateHomePage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            ProfileTutorViewModel model = new ProfileTutorViewModel
            {
                UserChannels = await GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming, "N|A"),
                UserLogins = await GetPopularStreamTutors(storageConfig),
            };
            return model;
        }

        public string FormatChatId(string chatID)
        {
            var formattedphrase = chatID.Split(new char[] { '\t' });
            var formattedChatID = formattedphrase[2].Split(new char[] { '\n' });
            return formattedphrase[1] + "|" + formattedChatID[0];
        }

        private async Task<List<UserLogin>> GetPopularStreamTutors([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            List<UserLogin> list = new List<UserLogin>();
            list = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, QueryHeaders.AllApprovedTutors.ToString(), null);
            return list;
        }

        //Saves picture into container on Azure - replaces old one if there is one
        public async Task<string> SaveIntoBlobContainer(IFormFile file, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user, string reference)
        {
            //Connects to blob storage and saves picture
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(reference);

            blockBlob.DeleteIfExists();

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    await file.CopyToAsync(ms);
                    blockBlob.UploadFromByteArray(ms.ToArray(), 0, (int)file.Length);
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<bool> SavePayment([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, Payment payment)
        {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", payment.Id } }, payment);
            return true;
        }

        public async Task<Payment> GetPayment([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string query, string txnID)
        {
            var payments = await DataStore.GetListAsync<Payment>(_connectionString, storageConfig.Value, query, new List<string> { txnID });
            if (payments.Count > 0) return payments[0];
            return null;
        }

        public async Task SaveDonationAttempt([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, DonationAttempt donationAttempt)
        {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", donationAttempt.Id } }, donationAttempt);
        }

        public async Task<bool> LogIPNRequest([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, IPNRequestBody request)
        {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", request.Id } }, request);
            return true;
        }

        //sends to any email from streamworktutor@gmail.com provided the 'from' 'to' 'subject' 'body' & 'attachments' (if needed)
        public async Task SendEmailToAnyEmailAsync(string from, string to, string subject, string body, List<Attachment> attachments)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("streamworktutor@gmail.com", "STREAMW0RK3R!"),
                EnableSsl = true
            };

            MailMessage message = new MailMessage();
            message.Subject = subject;
            message.To.Add(to);
            message.Body = body;
            message.From = new MailAddress(from);

            if (attachments != null)
            {
                foreach (var attachement in attachments)
                    message.Attachments.Add(attachement);
            }

            await client.SendMailAsync(message);
        }

        public string CreateUri(string username)
        {
            var uriBuilder = new UriBuilder("https://streamwork.live/Home/ChangePassword");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["username"] = username;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public async Task<string> GetChatSecretKey([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            var userChannel = await GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            var ids = userChannel[0].ChatId.Split("|");
            var encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username, "3O08UU-OtQ_rycx3")));
            var finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username + "&sig=" + encodedUrl;
            return finalString;
        }

        public string GetChatSecretKey(string tid, string tkey, string userName) //Overload for StreamPage
        {
            var encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + tid + "&tkey=" + tkey + "&nme=" + userName, "3O08UU-OtQ_rycx3")));
            var finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + tid + "&tkey=" + tkey + "&nme=" + userName + "&sig=" + encodedUrl;
            return finalString;
        }

        public byte[] hmacSHA256(string data, string key) //Encryption to get ChatSecretKey
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key)))
            {
                return hmac.ComputeHash(Encoding.ASCII.GetBytes(data));
            }
        }

        public string EncryptPassword(string password) //Hash for passwords
        {
            byte[] salt = new byte[128 / 8];
            using (var randomNumber = RandomNumberGenerator.Create())
            {
                randomNumber.GetBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, (256 / 8)));
            return hashed + "|" + Convert.ToBase64String(salt);
        }

        public string DecryptPassword(string salt, string password) //Compare Hash
        {
            var decrypt = salt.Split('|');
            var bytesSalt = Convert.FromBase64String(decrypt[1]);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, bytesSalt, KeyDerivationPrf.HMACSHA1, 10000, (256 / 8)));
            return hashed + "|" + decrypt[1];
        }
    }
}
