using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamHoster;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.StreamHoster;

namespace StreamWork.Services
{
    public class StreamService : StorageService
    {
        private DataModels.Channel channel;
        private Profile profile;
        private string streamTitle;
        private string streamSubject;
        private string streamDescription;
        private string streamThumbnail;
        private string archivedVideoId;
        private string streamColor;
        private string scheduleId;

        private int initialCount = 0;
        private static int threadCount = 0;
        private static Hashtable hashTable = new Hashtable();

        public StreamService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public string StartStream(HttpRequest request, Profile userProfile, DataModels.Channel userChannel)
        {
            try
            {
                var notifyStudent = request.Form["NotifiyStudent"];
                channel = userChannel;
                profile = userProfile;   
                streamTitle = request.Form["StreamTitle"];
                streamSubject = request.Form["StreamSubject"];
                streamDescription = request.Form["StreamDescription"];
                scheduleId = request.Form["ScheduleId"];
                archivedVideoId = Guid.NewGuid().ToString();
                streamColor = GetCorrespondingStreamColor(streamSubject);

                if (request.Form.Files.Count > 0)
                    streamThumbnail = BlobMethods.SaveImageIntoBlobContainer(request.Form.Files[0], archivedVideoId, 1280, 720);
                else
                    streamThumbnail = GetCorrespondingDefaultThumbnail(streamSubject);


                RunLiveThread();
                return archivedVideoId;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TutorMethods: StartStream " + e.Message);
                return null;
            }
        }

        public bool IsLive(string channelKey)
        {
            try
            {
                var response = CallJSON<StreamHosterEndpoint>("https://a.streamhoster.com/v1/papi/media/stream/stat/realtime-stream?targetcustomerid=" + channelKey, "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
                foreach (var channel in response.Data)
                {
                    if ((channel.MediaId + "_5").Equals(channelKey.Split("|")[0]))
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

        private bool RunLiveThread()
        {
            bool tryAPI = true;
            var cancellationToken = new CancellationToken();
            var channelIds = channel.ChannelKey;
            var channelKey = channelIds.Split("|")[0];

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    channel.StreamSubject = streamSubject;
                    channel.StreamTitle = streamTitle;
                    channel.StreamDescription = streamDescription;
                    channel.StreamThumbnail = streamThumbnail;
                    channel.Views = 0;
                    channel.StreamColor = streamColor;
                    channel.Name = profile.Name;
                    channel.ArchivedVideoId = archivedVideoId;
                    await DeleteFillScheduleTask();
                    await Save(channel.Id, channel);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e.Message);
                }

                while (tryAPI)
                {
                    await Task.Delay(5000, cancellationToken);
                    try
                    {
                        var response = IsLive(channel.ChannelKey);
                        if (response)
                            Console.WriteLine("Live");
                        else
                        {
                            Console.WriteLine("Not Live");
                            await Task.Delay(25000, cancellationToken);
                            await ClearChannelStreamInfo();
                            RunVideoThread();
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in RunLiveThread: " + ex.Message);
                        tryAPI = true;
                    }
                }
            }, TaskCreationOptions.LongRunning);

            return tryAPI;
        }

        private void RunVideoThread() //This thread handles checking if the stream is still live
        {
            bool tryAPI = true;
            var cancellationToken = new CancellationToken();
            var channelIds = channel.ChannelKey;
            var rssId = channelIds.Split("|")[1];

            var archivedVideo = GetVideo();
            threadCount += 1;
            hashTable.Add(threadCount, archivedVideo);

            StreamHosterRSSFeed initialResponse = CallXML<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/" + rssId + "?format=mrss");
            if (initialResponse.Channel.Item != null)
                initialCount = initialResponse.Channel.Item.Length;

            if (threadCount <= 1)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (tryAPI)
                    {
                        try
                        {
                            await Task.Delay(30000, cancellationToken);
                            StreamHosterRSSFeed response = CallXML<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/" + rssId + "?format=mrss");
                            if (response.Channel.Item != null && response.Channel.Item.Length == initialCount + threadCount)
                            {
                                Console.WriteLine("Videos Found");
                                await ArchiveVideo(response);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in RunVideoThread: " + ex.Message);
                            tryAPI = true;
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        private Video GetVideo()
        {
            Video archivedStream = new Video
            {
                Id = archivedVideoId,
                Username = channel.Username,
                StreamID = "",
                StreamTitle = streamTitle,
                StreamSubject = streamSubject,
                StreamDescription = streamDescription,
                StreamThumbnail = streamThumbnail,
                ProfilePicture = profile.ProfilePicture,
                StreamColor = GetCorrespondingStreamColor(streamSubject),
                Name = profile.Name
            };

            return archivedStream;
        }

        private async Task<bool> ArchiveVideo(StreamHosterRSSFeed response)
        {
            for (int i = 1; i < hashTable.Count + 1; i++)
            {
                var archivedStream = (Video)hashTable[i];
                archivedStream.StreamID = response.Channel.Item[threadCount - i].MediaContentId;
                archivedStream.Views = channel.Views;
                archivedStream.StartTime = DateTime.UtcNow;

                try
                {
                    await Save(archivedStream.Id, archivedStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetArchiveStream: " + ex.Message);
                }
            }

            threadCount = 0;
            initialCount = 0;
            hashTable.Clear();

            return true;
        }

        private async Task ClearChannelStreamInfo()
        {
            try
            {
                channel.StreamTitle = null;
                channel.StreamSubject = null;
                channel.StreamDescription = null;
                channel.StreamThumbnail = null;
                channel.StreamColor = null;
                channel.ArchivedVideoId = null;
                await Save(channel.Id, channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ClearChannelStreamInfo " + ex.Message);
            }
        }

        public string GetCorrespondingStreamColor(string subject)
        {
            Hashtable table = new Hashtable
            {
                { "Mathematics", "#AEE8FE" },
                { "Science", "#A29CFE" },
                { "Business", "#46A86E" },
                { "Engineering", "#74B9FF" },
                { "Law", "#F0AD4E" },
                { "Art", "#F8C5DC" },
                { "Humanities", "#FF7775" },
                { "Other", "#FECA6E" }
            };

            return (string)table[subject];
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

        private async Task DeleteFillScheduleTask()
        {
            if(scheduleId != null && scheduleId != "")
                await Delete<Schedule>(scheduleId);
        }
    }
}
