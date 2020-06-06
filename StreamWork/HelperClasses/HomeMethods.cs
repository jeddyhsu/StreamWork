using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public class HomeMethods {
        public static bool devEnvironment;
        public readonly string _host = devEnvironment ? "http://localhost:58539" : "https://www.streamwork.live";
        public readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";
        public readonly string _dacastAPIKey = "135034_9245336a05f4d4bdb6fa";
        public readonly string _defaultStreamHosterChannelKey = "Ec9jbSsc880_5";

        //Gets all user channels that are streaming
        public async Task<List<UserChannel>> GetAllUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, QueryHeaders.AllUserChannelsThatAreStreaming.ToString());
            return channels;
        }

        //Gets set of user channels with the query that you specify
        public async Task<List<UserChannel>> GetUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return channels;
        }

        //Gets a single user channel with the query that you specify
        public async Task<UserChannel> GetUserChannel([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (channels.Count > 0) return channels[0];
            return null;
        }

        //Gets all archived streams
        public async Task<List<UserArchivedStreams>> GetAllArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, QueryHeaders.AllArchivedVideos.ToString());
            return archivedStreams;
        }

        //Gets a set of archived streams with the query that you specify
        public async Task<List<UserArchivedStreams>> GetArchivedStreams ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return archivedStreams;
        }

        //Gets a single archived stream with the query that you specify
        public async Task<UserArchivedStreams> GetArchivedStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (archivedStreams.Count > 0) return archivedStreams[0];
            return null;
        }

        //Gets all user logins
        public async Task<List<UserLogin>> GetAllUserProfiles([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, QueryHeaders.AllSignedUpUsers.ToString());
            return logins;
        }

        //Gets a set of user logins with the query that you specify
        public async Task<List<UserLogin>> GetUserProfiles ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return logins;
        }

        //Gets a single user logins with the query that you specify
        public async Task<UserLogin> GetUserProfile ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, QueryHeaders query, string user) { //one user login information
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
            return await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, QueryHeaders.ApprovedTutorsByFollowers.ToString());
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
            var streamingUserChannels = await GetUserChannel(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming, "N|A");
            IndexViewModel model = new IndexViewModel();

            // List of streams for the carousel
            List<string> streamsWithPriority = new List<string> {
                "F8U3mEscyNB_1",
                "EBRNrFsAqZZ_1",
                "E1OKuVsAi9U_1",
                "EdowSgsAqJV_1",
                "EYd2jUscrUz_1",
                "Fr40wrscyQF_1",
                "ETWYvVscngb_1",
                "F5pYLrscQ5Q_1"
            }; // List of the IDs of the streams to hardcode in
            List<UserArchivedStreams> streamsByViews = await GetArchivedStreams(storageConfig, QueryHeaders.ArchivedStreamsByViews, null);
            List<UserArchivedStreams> userArchivedStreams = new List<UserArchivedStreams>();
            foreach (string streamWithPriority in streamsWithPriority) // Add hardcoded streams
            {
                int streamIndex = streamsByViews.FindIndex(x => x.StreamID.Equals(streamWithPriority));
                userArchivedStreams.Add(streamsByViews[streamIndex]);
                streamsByViews.RemoveAt(streamIndex);
            }
            int toAdd = 12 - userArchivedStreams.Count; // Since Count changes while the loop is running
            for (int i = 0; i < toAdd; i++) // Fill the rest in with streams in order of view count
            {
                userArchivedStreams.Add(streamsByViews[i]);
            }

            if (streamingUserChannels == null)
            {
                var userProfile= await GetUserProfile(storageConfig, QueryHeaders.CurrentUser, "juliamkim");
                var userChannel = await GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, "juliamkim");
                var getArchivedStreams = await GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, "juliamkim");

                model = new IndexViewModel
                {
                    UserLogin = userProfile,
                    UserChannel = userChannel,
                    UserArchivedStream = getArchivedStreams[0],
                    UserArchivedStreams = userArchivedStreams,
                };
            }
            else
            {
                var userProfileForChannel = await GetUserProfile(storageConfig, QueryHeaders.CurrentUser, streamingUserChannels.Username);
                 model = new IndexViewModel
                {
                    UserLogin = userProfileForChannel,
                    UserChannel = streamingUserChannels,
                    UserArchivedStreams = userArchivedStreams,
                };
            }

            return model;
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
                    int currWidth = image.Width;
                    int currHeight = image.Height;
                    if ((float) width / height > (float) currWidth / currHeight) // image is too tall relative to its width
                    {
                        int resizeHeight = (int) ((float) currHeight / currWidth * width);
                        image.Mutate(i => i.Resize(width, resizeHeight).Crop(new Rectangle(0, Math.Abs(resizeHeight - height) / 2, width - 1, height - 1)));
                    }
                    else // image is too wide relative to its height, or perfect
                    {
                        int resizeWidth = (int) ((float) currWidth / currHeight * height);
                        image.Mutate(i => i.Resize(resizeWidth, height).Crop(new Rectangle(Math.Abs(resizeWidth - width) / 2, 0, width - 1, height - 1)));
                    }
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

        public string EncryptString(string s)
        {
            if(s != null || s != "")
            {
                byte[] bArray = Encoding.ASCII.GetBytes(s);
                string encrypted = Convert.ToBase64String(bArray);
                return encrypted;
            }

            return null;
        }

        public string DecryptString(string s)
        {
            byte[] bArray;
            if (s != null && s != "")
            {
                bArray = Convert.FromBase64String(s);
                return Encoding.ASCII.GetString(bArray);
            }

            return null;
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

        public string GetRandomChatColor()
        {
            var random = new Random();
            var list = new List<string> { "#D9534F", "#F0AD4E", "#56C0E0", "#5CB85C", "#1C7CD5", "#8B4FD9"};
            int index = random.Next(list.Count);
            return list[index];
        }
    }
}
