using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.StreamHoster;
using StreamWork.Threads;

namespace StreamWork.Services
{
    public class StreamService : StorageService
    {
        public StreamService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public bool StartStream(HttpRequest request, UserLogin userProfile, UserChannel userChannel)
        {
            try
            {
                string streamThumbnail;
                var streamTitle = request.Form["StreamTitle"];
                var streamSubject = request.Form["StreamSubject"];
                var streamDescription = request.Form["StreamDescription"];
                var notifyStudent = request.Form["NotifiyStudent"];
                var archivedStreamId = Guid.NewGuid().ToString();
                if (request.Form.Files.Count > 0)
                    streamThumbnail = BlobMethods.SaveImageIntoBlobContainer(request.Form.Files[0], archivedStreamId, 1280, 720);
                else
                    streamThumbnail = GetCorrespondingDefaultThumbnail(streamSubject);

                StreamClient streamClient = new StreamClient(config, userProfile, userChannel, streamTitle, streamSubject, streamDescription, streamThumbnail, archivedStreamId, "#8B4FD9");
                //if (notifyStudent.Equals("yes")) streamClient.RunEmailThread();
                streamClient.RunLiveThread();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TutorMethods: StartStream " + e.Message);
                return false;
            }
        }

        public bool IsLive(string channelKey)
        {
            try
            {
                var response = Call<StreamHosterEndpoint>("https://a.streamhoster.com/v1/papi/media/stream/stat/realtime-stream?targetcustomerid=" + channelKey, "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
                foreach (var channel in response.Data)
                {
                    if (channel.MediaId == channelKey.Split("|")[0])
                    {
                        Console.WriteLine("Live");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Not Live");
                    }
                }

                return false;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Error in IsLive: " + ex.Message);
                return false;
            }
        }

        private string GetCorrespondingDefaultThumbnail(string subject)
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

    }
}
