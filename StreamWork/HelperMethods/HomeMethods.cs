using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.Models;
using StreamWork.ViewModels;

namespace StreamWork.HelperMethods
{
    public class HomeMethods
    {
        public static bool devEnvironment;
        public readonly string _host = devEnvironment ? "http://localhost:58539" : "https://www.streamwork.live";
        public readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public readonly string _dacastAPIKey = "135034_9245336a05f4d4bdb6fa";
        public readonly string _defaultStreamHosterChannelKey = "Ec9jbSsc880_5";
        public readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();

        //Gets all user channels that are streaming
        public async Task<List<UserChannel>> GetAllUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, SQLQueries.GetAllUserChannelsThatAreStreaming.ToString());
            return channels;
        }

        //Gets set of user channels with the query that you specify
        public async Task<List<UserChannel>> GetUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, SQLQueries query, string user)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return channels;
        }

        //Gets a single user channel with the query that you specify
        public async Task<UserChannel> GetUserChannel([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, SQLQueries query, string user)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (channels.Count > 0) return channels[0];
            return null;
        }

        //Gets all archived streams
        public async Task<List<UserArchivedStreams>> GetAllArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, SQLQueries.GetAllArchivedStreams.ToString());
            return archivedStreams;
        }

        //Gets a set of archived streams with the query that you specify
        public async Task<List<UserArchivedStreams>> GetArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, SQLQueries query, string user)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return archivedStreams;
        }

        //Gets a single archived stream with the query that you specify
        public async Task<UserArchivedStreams> GetArchivedStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, SQLQueries query, string user)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (archivedStreams.Count > 0) return archivedStreams[0];
            return null;
        }

        //Gets all user logins
        public async Task<List<UserLogin>> GetAllUserProfiles([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, SQLQueries.GetAllUsers.ToString());
            return logins;
        }

        //Gets a set of user logins with the query that you specify
        public async Task<List<UserLogin>> GetUserProfiles([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, SQLQueries query, string user)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            return logins;
        }

        //Gets a single user logins with the query that you specify
        public async Task<UserLogin> GetUserProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, SQLQueries query, string user)
        { //one user login information
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query.ToString(), new List<string> { user });
            if (logins.Count > 0) return logins[0];
            return null;
        }

        public async Task<List<UserArchivedStreams>> GetPreviouslyWatchedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string studentName)
        {
            var previousViews = await DataStore.GetListAsync<View>(_connectionString, storageConfig.Value, SQLQueries.GetViewsWithViewer.ToString(), new List<string> { studentName });
            if (previousViews.Count == 0) return null;
            List<string> idList = new List<string>();
            foreach (var view in previousViews) idList.Add(view.StreamId);
            return await GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsByStreamIdInTheList, FormatQueryString(idList));
        }

        public async Task UpdateUser([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin user)
        {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } }, user);
        }

        public async Task<List<UserLogin>> GetPopularStreamTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, SQLQueries.GetApprovedTutorsByFollowers.ToString());
        }

        public async Task<List<UserChannel>> SearchUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, SQLQueries.GetAllUserChannelsThatAreStreaming.ToString(), new List<string> { "" });
                return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, SQLQueries.GetUserChannelsBySearchTerm.ToString(), new List<string> { searchQuery.ToLower() });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, SQLQueries.GetUserChannelWithSubject.ToString(), new List<string> { subject });
                return await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, SQLQueries.GetUserChannelsBySubjectAndSearchTerm.ToString(), new List<string> { subject, searchQuery.ToLower() });
            }
        }

        public async Task<List<UserArchivedStreams>> SearchArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, SQLQueries.GetAllArchivedStreams.ToString());
                return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, SQLQueries.GetArchivedStreamsWithSearchTerm.ToString(), new List<string> { searchQuery.ToLower() });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery)) return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, SQLQueries.GetArchivedStreamsWithSubject.ToString(), new List<string> { subject });
                return await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, SQLQueries.GetArchivedStreamsWithSubjectAndSearchTerm.ToString(), new List<string> { subject, searchQuery.ToLower() });
            }
        }

        public string GetSubjectIcon(string subject)
        {
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

            foreach (string pic in key)
            {
                if (pic == subject)
                {
                    defaultURL = ((string)defaultPic[pic]);
                }
            }

            return defaultURL;
        }

        public async Task<IndexViewModel> PopulateHomePage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string userName, bool isAutheticated)
        {
            var streamingUserChannels = await GetUserChannel(storageConfig, SQLQueries.GetAllUserChannelsThatAreStreaming, "N|A");
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
            };

            // List of the IDs of the streams to hardcode in
            List<UserArchivedStreams> streamsByViews = await GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsInDescendingOrderByViews, null);
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
                var userProfile = await GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, "juliamkim");
                var userChannel = await GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, "juliamkim");
                var getArchivedStreams = await GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, "juliamkim");

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
                var userProfileForChannel = await GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, streamingUserChannels.Username);
                model = new IndexViewModel
                {
                    UserLogin = userProfileForChannel,
                    UserChannel = streamingUserChannels,
                    UserArchivedStreams = userArchivedStreams,
                };
            }

            if (isAutheticated)
            {
                var userProfile = await GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, userName);
                model.GenericUserProfile = userProfile;
                model.ChatInfo = _encryptionMethods.EncryptString(userProfile.Username + "|" + userProfile.Id + "|" + userProfile.EmailAddress);
            }

            return model;
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

        public string CreateUri(string username, string key)
        {
            var uriBuilder = new UriBuilder(_host + "/Home/ChangePassword");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["username"] = username;
            query["key"] = key;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }


        public async Task<List<Recommendation>> GetRecommendationsForTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string tutor)
        {
            return await DataStore.GetListAsync<Recommendation>(_connectionString, storageConfig.Value, SQLQueries.GetRecommendationsWithTutorUsername.ToString(), new List<string> { tutor });
        }

        public async Task SaveRecommendation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string student, string tutor, string text)
        {
            Recommendation recommendation = new Recommendation
            {
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
            var list = new List<string> { "#D9534F", "#F0AD4E", "#56C0E0", "#5CB85C", "#1C7CD5", "#8B4FD9" };
            int index = random.Next(list.Count);
            return list[index];
        }

        public string GetCorrespondingSubjectThumbnail(string subject)
        {
            Hashtable table = new Hashtable();
            table.Add("Mathematics", "/images/ChatAssets/Math.png");
            table.Add("Science", "/images/ChatAssets/Science.png");
            table.Add("Business", "/images/ChatAssets/Business.png");
            table.Add("Engineering", "/images/ChatAssets/Engineering.png");
            table.Add("Law", "/images/ChatAssets/Law.png");
            table.Add("Art", "/images/ChatAssets/Art.png");
            table.Add("Humanities", "/images/ChatAssets/Humanities.png");
            table.Add("Other", "/images/ChatAssets/Other.png");


            return (string)table[subject];
        }
    }
}
