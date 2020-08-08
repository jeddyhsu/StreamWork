using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.ProfileObjects;

namespace StreamWork.Services
{
    public class ProfileService : StorageService
    {
        const string EOL = "|__*%ENDOFLINE%*__|";
        const string DELIMITER = "|__*%SPLIT%*__|";

        public ProfileService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public bool SaveSection(HttpRequest request, Profile userProfile)
        {
            try
            {
                var form = request.Form;
                var keys = form.Keys;

                string formatString = "";
                foreach (var key in keys)
                {
                    formatString += key.ToString() + DELIMITER + form[key] + EOL;
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

        public List<Section> GetSections(Profile userProfile)
        {
            try
            {
                List<Section> sectionsList = new List<Section>();

                var blob = BlobMethods.GetBlockBlob(userProfile.Username + "-" + userProfile.Id + "-sections" + ".txt");
                var sections = blob.DownloadText();
                var sectionsSplit = sections.Split(EOL);

                if (sectionsSplit[0].Split(DELIMITER)[1] != "")
                {
                    sectionsList.Add(new Section(sectionsSplit[0].Split(DELIMITER)[1]));
                }

                for (int i = 1; i < sectionsSplit.Length - 1; i += 2)
                {
                    var title = sectionsSplit[i].Split(DELIMITER)[1];
                    var description = sectionsSplit[i + 1].Split(DELIMITER)[1];

                    if (!title.Equals("") || !description.Equals("") || i <= 1)
                    {
                        description = description.Replace("*--*", Environment.NewLine);
                        sectionsList.Add(new Section(title, description, description.Split(" ").Length > 66));
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

        public bool SaveTopic(HttpRequest request, Profile userProfile)
        {
            try
            {
                var form = request.Form;
                var keys = form.Keys;

                string formatString = "";
                foreach (var key in keys)
                {
                    formatString += key.ToString() + DELIMITER + form[key] + EOL;
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

        public List<Topic> GetTopics(Profile userProfile)
        {
            try
            {
                List<Topic> topicsList = new List<Topic>();

                var blob = BlobMethods.GetBlockBlob(userProfile.Username + "-" + userProfile.Id + "-topics" + ".txt");
                var topics = blob.DownloadText();
                var topicSplit = topics.Split(EOL);

                for (int i = 0; i < topicSplit.Length - 1; i += 2)
                {
                    var topic = topicSplit[i].Split(DELIMITER)[1];
                    var listOfSubjects = topicSplit[i + 1].Split(DELIMITER)[1];

                    if (!topic.Equals("") || !listOfSubjects.Equals("") || (i <= 1))
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

        public async Task<List<string>> SaveEditedArchivedStream(HttpRequest request)
        {
            string streamThumbnail = null;
            var videoId = request.Form["StreamId"];
            var streamTitle = request.Form["StreamTitle"];
            var streamDescription = request.Form["StreamDescription"];
            if (request.Form.Files.Count > 0)
                streamThumbnail = BlobMethods.SaveImageIntoBlobContainer(request.Form.Files[0], videoId, 1280, 720);

            var archivedStream = await Get<Video>(SQLQueries.GetArchivedStreamsWithId, videoId);
            archivedStream.StreamTitle = streamTitle;
            archivedStream.StreamDescription = streamDescription;
            if (streamThumbnail != null)
                archivedStream.StreamThumbnail = streamThumbnail;

            await Save<Video>(archivedStream.Id, archivedStream);

            return new List<string> { streamTitle, streamDescription, archivedStream.StreamThumbnail };
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
            var allArchivedStreamsByUser = await GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { user });
            var userChannel = await Get<Channel>(SQLQueries.GetUserChannelWithUsername, new string[] { user });
            foreach (var stream in allArchivedStreamsByUser)
            {
                stream.ProfilePicture = profilePicture;
                await Save(stream.Id, stream);
            }
            userChannel.ProfilePicture = profilePicture;
            await Save(userChannel.Id, userChannel);
        }

        public async Task<bool> DeleteStream(string id)
        {
            return await Delete<Video>(id);
        }

        public async Task<bool> ChangeColor(Profile userProfile, string color) {
            userProfile.ProfileColor = color;

            return await Save(userProfile.Id, userProfile);
        }
    }
}
