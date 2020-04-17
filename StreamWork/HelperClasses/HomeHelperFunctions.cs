using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace StreamWork.HelperClasses
{
    public class HomeHelperFunctions {
        public static bool devEnvironment;
        public readonly string _host = devEnvironment ? "http://localhost:58539" : "https://www.streamwork.live";
        public readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";
        public readonly string _dacastAPIKey = "135034_9245336a05f4d4bdb6fa";
        public readonly string _defaultStreamHosterChannelKey = "Ec9jbSsc880_5";

        //Gets set of userchannels with the query that you specify
        public async Task<List<UserChannel>> GetUserChannels ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return channels;
        }

        //All user channels
        public async Task<List<UserChannel>> GetUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString());
            return channels;
        }

        public async Task<List<UserChannel>> GetUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, List<string> parameters)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), parameters);
            return channels;
        }

        //Gets a set of archived streams with the query that you specify
        public async Task<List<UserArchivedStreams>> GetArchivedStreams ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return archivedStreams;
        }

        //All Archived Streams
        public async Task<List<UserArchivedStreams>> GetArchivedStreams ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query) {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString());
            return archivedStreams;
        }

        //Gets a set of user logins with the query that you specify
        public async Task<List<UserLogin>> GetUserLogins ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return logins;
        }

        //Gets a set of user logins with the query that you specify
        public async Task<List<UserLogin>> GetUserLogins([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString());
            return logins;
        }

        public async Task<UserLogin> GetUserProfile ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (logins.Count > 0) return logins[0];
            return null;
        }

        public async Task<List<UserArchivedStreams>> GetPreviouslyWatchedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string studentName)
        {
            var previousViews = await DataStore.GetListAsync<View>(_connectionString, storageConfig.Value, QueryHeaders.ViewsByViewer.ToString(), new List<string> { studentName });
            if (previousViews.Count == 0) return null;
            List<string> idList = new List<string>();
            foreach (var view in previousViews) idList.Add(view.StreamId);
            return await GetArchivedStreams(storageConfig, QueryHeaders.MultipleArchivedVideosByStreamId, FormatQueryString(idList));
        }

        public async Task UpdateUser ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin user) {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } }, user);
        }

        public async Task<List<UserLogin>> GetPopularStreamTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, QueryHeaders.AllApprovedTutors.ToString());
        }

        public async Task<List<UserChannel>> SearchUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, QueryHeaders.AllUserChannelsThatAreStreaming.ToString(), new List<string> { "" });
                return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, QueryHeaders.UserChannelsBySearchTerm.ToString(), new List<string> { searchQuery.ToLower() });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, QueryHeaders.AllUserChannelsThatAreStreamingWithSpecifiedSubject.ToString(), new List<string> { subject });
                return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, QueryHeaders.UserChannelsBySubjectAndSearchTerm.ToString(), new List<string> { subject, searchQuery.ToLower() });
            }
        }

        public async Task<List<UserArchivedStreams>> SearchArchivedStreams ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, QueryHeaders.AllArchivedVideos.ToString());
                return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, QueryHeaders.ArchivedVideosBySearchTerm.ToString(), new List<string> { searchQuery.ToLower() });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, QueryHeaders.UserArchivedVideosBasedOnSubject.ToString(), new List<string> { subject });
                return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, QueryHeaders.ArchivedVideosBySubjectAndSearchTerm.ToString(), new List<string> { subject, searchQuery.ToLower() });
            }
        }
       
        public string GetSubjectIcon (string subject) {
            string defaultURL = "";

            Hashtable defaultPic = new Hashtable
            {
                { "Mathematics", "/images/mathematics.svg" },
                { "Humanities", "/images/book.svg" },
                { "Science", "/images/microscope.svg" },
                { "Business", "/images/business-strategy.svg" },
                { "Engineering", "/images/gear.svg" },
                { "Law", "/images/court-gavel.svg" },
                { "Art", "/images/inclilned-paint-brush.svg" },
                { "Other", "/images/eye.svg" }
            };

            ICollection key = defaultPic.Keys;

            foreach (string pic in key) {
                if (pic == subject) {
                    defaultURL = ((string)defaultPic[pic]);
                }
            }

            return defaultURL;
        }

        public async Task<IndexViewModel> PopulateHomePage ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string currentUser) {
            var streamingUserChannels = await GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming, "N|A");
            if (streamingUserChannels.Count == 0)
            {
                var userLogin = await GetUserLogins(storageConfig, QueryHeaders.CurrentUser, "admin");
                var userChannel = await GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, "admin");
                var getArchivedStreams = await GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, "admin");

                IndexViewModel model = new IndexViewModel
                {
                    UserLogin = userLogin[0],
                    UserChannel = userChannel[0],
                    UserArchivedStream = getArchivedStreams[0],
                    UserArchivedStreams = await GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos, null),
                    ChatBox = await GetChatSecretKey(storageConfig, userChannel[0].ChatId, currentUser)
                };

                model.UserArchivedStreams = model.UserArchivedStreams.Skip(model.UserArchivedStreams.Count - 4).ToList();

                return model;
            }

            var userLoginForChannel = await GetUserLogins(storageConfig, QueryHeaders.CurrentUser, streamingUserChannels[0].Username);
            IndexViewModel channelModel = new IndexViewModel
            {
                UserLogin = userLoginForChannel[0],
                UserChannel = streamingUserChannels[0],
                UserArchivedStreams = await GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos, null),
                ChatBox = await GetChatSecretKey(storageConfig, streamingUserChannels[0].ChatId, currentUser)
            };

            channelModel.UserArchivedStreams = channelModel.UserArchivedStreams.Skip(channelModel.UserArchivedStreams.Count - 4).ToList();

            return channelModel;
        }

        public string FormatChatId (string chatID) {
            var formattedphrase = chatID.Split(new char[] { '\t' });
            var formattedChatID = formattedphrase[2].Split(new char[] { '\n' });
            return formattedphrase[1] + "|" + formattedChatID[0];
        }

        //Saves picture into container on Azure - replaces old one if there is one
        public string SaveIntoBlobContainer(IFormFile file, string reference, int width, int height) {

            //Connects to blob storage and saves picture
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(reference);
            blockBlob.DeleteIfExists();

            var encoder = new PngEncoder();

            using (var stream = file.OpenReadStream())
            {
                using (var output = new MemoryStream())
                using (Image image = Image.Load(stream))
                {
                    image.Mutate(x => x.Resize(width, height));
                    image.Save(output, encoder);
                    output.Position = 0;
                    blockBlob.UploadFromStream(output);
                }
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<bool> SavePayment ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, Payment payment) {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", payment.Id } }, payment);
            return true;
        }

        public async Task<Payment> GetPayment ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string query, string txnID) {
            var payments = await DataStore.GetListAsync<Payment>(_connectionString, storageConfig.Value, query, new List<string> { txnID });
            if (payments.Count > 0) return payments[0];
            return null;
        }

        public string CreateUri (string username, string key) {
            var uriBuilder = new UriBuilder(_host + "/Home/ChangePassword");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["username"] = username;
            query["key"] = key;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public async Task<string> GetChatSecretKey([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId, string userName) //Overload for StreamPage
        {
            var split = chatId.Split("|");
            var tid = split[0];
            var tkey = split[1];
            if (userName != null)
            {
                var userLogin = (await GetUserLogins(storageConfig, QueryHeaders.CurrentUser, userName))[0];
                var _encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + tid + "&tkey=" + tkey + "&nme=" + userLogin.Name.Replace('|','_') + "&pic=" + userLogin.ProfilePicture, "3O08UU-OtQ_rycx3")));
                var _finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + tid + "&tkey=" + tkey + "&nme=" + userLogin.Name.Replace('|', '_') + "&pic=" + userLogin.ProfilePicture + "&sig=" + _encodedUrl;
                return _finalString;
            }

            var encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + tid + "&tkey=" + tkey + "&nme=" + userName, "3O08UU-OtQ_rycx3")));
            var finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + tid + "&tkey=" + tkey + "&nme=" + userName + "&sig=" + encodedUrl;
            return finalString;
        }

        public byte[] hmacSHA256 (string data, string key) //Encryption to get ChatSecretKey
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key))) {
                return hmac.ComputeHash(Encoding.ASCII.GetBytes(data));
            }
        }

        public string EncryptPassword (string password) //Hash for passwords
        {
            byte[] salt = new byte[128 / 8];
            using (var randomNumber = RandomNumberGenerator.Create()) {
                randomNumber.GetBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, (256 / 8)));
            return hashed + "|" + Convert.ToBase64String(salt);
        }

        public string DecryptPassword (string salt, string password) //Compare Hash
        {
            var decrypt = salt.Split('|');
            var bytesSalt = Convert.FromBase64String(decrypt[1]);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, bytesSalt, KeyDerivationPrf.HMACSHA1, 10000, (256 / 8)));
            return hashed + "|" + decrypt[1];
        }

        public async Task<List<Recommendation>> GetRecommendationsForTutor ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string tutor) {
            return await DataStore.GetListAsync<Recommendation>(_connectionString, storageConfig.Value, QueryHeaders.RecommendationsByTutor.ToString(), new List<string> {tutor});
        }

        public async Task SaveRecommendation ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string student, string tutor, string text) {
            Recommendation recommendation = new Recommendation {
                Id = Guid.NewGuid().ToString(),
                Student = student,
                Tutor = tutor,
                Text = text,
            };
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", recommendation.Id } }, recommendation);
        }

        public string FormatQueryString(List<string> list)
        {
            if (list.Count == 0) return null;
            string ids = "";
            foreach (string id in list) ids += "'" + id + "'" + ",";
            ids = ids.Remove(0, 1);
            ids = ids.Remove(ids.Length - 2, 2);

            return ids;
        }
    }
}
