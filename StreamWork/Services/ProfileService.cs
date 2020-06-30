using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Threads;
using StreamWork.TutorObjects;

namespace StreamWork.Services
{
    public class ProfileService : StorageService
    {
        public ProfileService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }
        
         //Going to simplify this code once the whole refractor is done
        public bool SaveSection(HttpRequest request, UserLogin userProfile)
        {
            try
            {
                var form = request.Form;
                var keys = form.Keys;

                string formatString = "";
                foreach (var key in keys)
                {
                    if (!form[key].Equals(""))
                        formatString += key.ToString() + "|``~``|" + form[key] + Environment.NewLine;
                }

                var url = BlobMethods.SaveFileIntoBlobContainer(userProfile.Username + "-" + userProfile.Id + "-sections" + ".txt", formatString);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TutorMethods: SaveSection " + e.Message);
                return false;
            }
        }

        public List<Section> GetSections(UserLogin userProfile)
        {
            try
            {
                List<Section> sectionsList = new List<Section>();

                var blob = BlobMethods.GetBlockBlob(userProfile.Username + "-" + userProfile.Id + "-sections" + ".txt");
                var sections = blob.DownloadText();
                var sectionsSplit = sections.Split(Environment.NewLine);

                sectionsList.Add(new Section(sectionsSplit[0].Split("|``~``|")[1]));

                for (int i = 1; i < sectionsSplit.Length - 1; i += 2)
                {
                    var title = sectionsSplit[i].Split("|``~``|")[1];
                    var description = sectionsSplit[i + 1].Split("|``~``|")[1];

                    if (!title.Equals("") || !description.Equals("") || i <= 1)
                    {
                        description = description.Replace("*--*", Environment.NewLine);
                        sectionsList.Add(new Section(title, description));
                    }
                }
                return sectionsList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TutorMethods: GetSections " + e.Message);
                return null;
            }

        }

        public bool SaveTopic(HttpRequest request, UserLogin userProfile)
        {
            try
            {
                var form = request.Form;
                var keys = form.Keys;

                string formatString = "";
                foreach (var key in keys)
                {
                    if (!form[key].Equals(""))
                        formatString += key.ToString() + "|``~``|" + form[key] + Environment.NewLine;
                }

                var url = BlobMethods.SaveFileIntoBlobContainer(userProfile.Username + "-" + userProfile.Id + "-topics" + ".txt", formatString);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TutorMethods: SaveTopic " + e.Message);
                return false;
            }
        }

        public List<Topic> GetTopics(UserLogin userProfile)
        {
            try
            {
                List<Topic> topicsList = new List<Topic>();

                var blob = BlobMethods.GetBlockBlob(userProfile.Username + "-" + userProfile.Id + "-topics" + ".txt");
                var topics = blob.DownloadText();
                var topicSplit = topics.Split(Environment.NewLine);

                for (int i = 0; i < topicSplit.Length - 1; i += 2)
                {
                    var topic = topicSplit[i].Split("|``~``|")[1];
                    var listOfSubjects = topicSplit[i + 1].Split("|``~``|")[1];

                    if (!topic.Equals("") || !listOfSubjects.Equals(""))
                    {
                        listOfSubjects = listOfSubjects.Replace("*--*", Environment.NewLine);
                        topicsList.Add(new Topic(topic, listOfSubjects));
                    }
                }
                return topicsList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TutorMethods: GetTopics " + e.Message);
                return null;
            }

        }

        public async Task<string[]> EditArchivedStream(HttpRequest request)
        {
            string streamThumbnail = null;
            var videoId = request.Form["StreamId"];
            var streamTitle = request.Form["StreamTitle"];
            var streamDescription = request.Form["StreamDescription"];
            if (request.Form.Files.Count > 0)
                streamThumbnail = BlobMethods.SaveImageIntoBlobContainer(request.Form.Files[0], videoId, 1280, 720);

            var archivedStream = await Get<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithId, videoId);
            archivedStream.StreamTitle = streamTitle;
            archivedStream.StreamDescription = streamDescription;
            if (streamThumbnail != null)
                archivedStream.StreamThumbnail = streamThumbnail;

            await Save<UserArchivedStreams>(archivedStream.Id, archivedStream);

            return new string[] { streamTitle, streamDescription, archivedStream.StreamThumbnail };
        }

        //Uses a hashtable to add default thumbnails based on subject
        public string GetCorrespondingDefaultThumbnail(string subject)
        {
            Hashtable defaultPic = new Hashtable
            {
                { "Mathematics", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/MathDefault.png" },
                { "Humanities", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/HumanitiesDefault.png" },
                { "Science", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/ScienceDefault.png" },
                { "Business", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/BusinessDefault.png" },
                { "Engineering", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/EngineeringDefault.png" },
                { "Law", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/LawDefault.png" },
                { "Art", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/ArtDefault.png" },
                { "Other", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/OtherDefualt.png" }
            };

            return (string)defaultPic[subject];
        }

        public async Task ChangeAllArchivedStreamAndUserChannelProfilePhotos(string user, string profilePicture) //changes all profile photos on streams if user has changed it
        {
            var allArchivedStreamsByUser = await GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { user });
            var userChannel = await Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, new string[] { user });
            foreach (var stream in allArchivedStreamsByUser)
            {
                stream.ProfilePicture = profilePicture;
                await Save<UserChannel>(stream.Id, stream);
            }
            userChannel.ProfilePicture = profilePicture;
            await Save<UserChannel>(userChannel.Id, userChannel);
        }

        public async Task DeleteStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await DataStore.DeleteAsync<UserArchivedStreams>(connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", id } });
        }
    }
}
