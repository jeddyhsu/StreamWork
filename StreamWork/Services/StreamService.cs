using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using StreamHoster;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.StreamHoster;

namespace StreamWork.Services
{
    public class StreamService : StorageService
    {
        ILogger log;

        private static ConcurrentDictionary<string, List<Video>> streamHandler = new ConcurrentDictionary<string, List<Video>>(); //concurrent dict will provide a pipeline where we can store username associated with the videos that they are streaming (thread safe)

        public StreamService([FromServices] IOptionsSnapshot<StorageConfig> config, ILogger logger) : base(config) { log = logger; }

        public async Task<string> StartStream(HttpRequest request, Profile profile)
        {
            try
            {
                var channel = await Get<DataModels.Channel>(SQLQueries.GetUserChannelWithUsername, profile.Username);
                channel.StreamTitle = request.Form["StreamTitle"]; //save all information in database from client side request
                channel.StreamSubject = request.Form["StreamSubject"];
                channel.StreamDescription = request.Form["StreamDescription"];
                channel.Views = 0;
                channel.StreamColor = GetCorrespondingStreamColor(request.Form["StreamSubject"]);
                channel.Name = profile.Name;
                channel.ArchivedVideoId = Guid.NewGuid().ToString();
                channel.InitialStreamCount = 0;
                await DeleteFillScheduleTask(request.Form["ScheduleId"]);
                
                if (request.Form.Files.Count > 0)
                    channel.StreamThumbnail =  BlobMethods.SaveImageIntoBlobContainer(request.Form.Files[0], channel.ArchivedVideoId, 1280, 720);
                else
                    channel.StreamThumbnail = GetCorrespondingDefaultThumbnail(channel.StreamSubject);

                await Save(channel.Id, channel);
                await RunLive(channel);
                return channel.ArchivedVideoId;
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
                }

                return false;
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error(ex, "Error in IsLive Method");
                return false;
            }
        }

        private async Task StartRecording(DataModels.Channel channel)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
            var resp = await client.PostAsync("https://a.streamhoster.com/v1/papi/media/stream/" + channel.ChannelKey.Remove(channel.ChannelKey.Length - 2) + "/record", new StringContent("{\"recordMode\":\"manual_on\"}", Encoding.UTF8, "application/json"));
            await resp.Content.ReadAsByteArrayAsync();
        }

        private async Task StopRecording(DataModels.Channel channel)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
            var resp = await client.PostAsync("https://a.streamhoster.com/v1/papi/media/stream/" + channel.ChannelKey.Remove(channel.ChannelKey.Length - 2) + "/record", new StringContent("{\"recordMode\":\"manual_off\"}", Encoding.UTF8, "application/json"));
            await resp.Content.ReadAsByteArrayAsync();
        }

        private async Task RunLive(DataModels.Channel channel)
        {
            var cancellationToken = new CancellationToken();

            await Task.Factory.StartNew(async () =>
             {
                 await StartRecording(channel);

                 while (true)
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
                             await StopRecording(channel);
                             await Task.Delay(5000, cancellationToken);
                             await PollVideo(channel);
                             break;
                         }
                     }
                     catch (Exception ex)
                     {
                         log.Error(ex, "Error in RunLive Method");
                     }
                 }
                 log.Write(Serilog.Events.LogEventLevel.Information, "Thread Finished|" + channel.Username);
                 Console.WriteLine("Thread Finished|" + channel.Username);
             }, TaskCreationOptions.LongRunning);
        }

        private async Task PollVideo(DataModels.Channel channel) //This thread handles checking if the stream is still live
        {
            var archivedVideo = GetVideo(channel); //Get video information from channel

            await ClearChannelStreamInfo(channel); //clear all channel info

            if (streamHandler.ContainsKey(channel.Username)) //check if the concurrent dict has the key with the specified username, if it does this means that a thread has already been spawned looking for this video in the rss feed
            {
                List<Video> oldvideos = new List<Video>();
                List<Video> videos = new List<Video>();
                streamHandler.TryGetValue(channel.Username, out videos);  //add the video info to the list of videos in the concurrent dictionary
                streamHandler.TryGetValue(channel.Username, out oldvideos);
                videos.Add(archivedVideo);
                streamHandler.TryUpdate(channel.Username, videos, oldvideos);
            }
            else
            {
                streamHandler.TryAdd(channel.Username, new List<Video> { archivedVideo });//create a new list and add the archived video info into the list
                StreamHosterRSSFeed initialResponse = CallXML<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/iAahx7oSswv?format=mrss"); //feed contains all the archived videos and we use it to get the stream id for each video
                if (initialResponse.Channel.Item != null)
                {
                    channel.InitialStreamCount = GetNumberOfVideosWithId(channel.ChannelKey, initialResponse); //check for initial count of the rss feed
                    await Save(channel.Id, channel);
                }

                while (true) //looking for the initial count of the feed + how many items are in the video list. The video list is retreived by the username of the current user
                {
                    await Task.Delay(30000);
                    StreamHosterRSSFeed response = CallXML<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/iAahx7oSswv?format=mrss");
                    List<Video> videos = new List<Video>();
                    streamHandler.TryGetValue(channel.Username, out videos);

                    Debug d = new Debug();
                    d.Id = Guid.NewGuid().ToString();
                    d.Message = channel.Username + "|" + (GetNumberOfVideosWithId(channel.ChannelKey, response));
                    await Save(d.Id, d);

                    log.Write(Serilog.Events.LogEventLevel.Information, channel.Username + " is still in the while loop");

                    if (response.Channel.Item != null && (GetNumberOfVideosWithId(channel.ChannelKey, response) >= channel.InitialStreamCount + videos.Count)) // we keep polling until the feed is updated to the number of videos (the initial count & concurrent dict count)
                    {
                        Debug d1 = new Debug();
                        d1.Id = Guid.NewGuid().ToString();
                        d1.Message = channel.Username + "Videos Found";
                        await Save(d1.Id, d1);

                        Console.WriteLine("Videos Found");
                        await ArchiveVideo(response, channel);
                        break;
                    }
                }
            }
        }

        private async Task ArchiveVideo(StreamHosterRSSFeed response, DataModels.Channel channel)
        {

            List<Video> videos = new List<Video>();
            streamHandler.TryGetValue(channel.Username, out videos);
            var ids = GetVideoIdsWithId(channel.ChannelKey, videos.Count, response);
            videos.Reverse(); //reverse list content

            for (int i = 0; i < videos.Count; i++) //go through the response as well as the dict and associate each stream to its object
            {
                var video = videos[i];
                video.StreamID = ids[i];
                video.Views = channel.Views;
                await Save(video.Id, video);
            }

            channel.InitialStreamCount = 0;
            await Save(channel.Id, channel);
            streamHandler.TryRemove(channel.Username, out _);
        }

        private Video GetVideo(DataModels.Channel channel)
        {
            Video archivedStream = new Video
            {
                Id = channel.ArchivedVideoId,
                Username = channel.Username,
                StreamID = "",
                StreamTitle = channel.StreamTitle,
                StreamSubject = channel.StreamSubject,
                StreamDescription = channel.StreamDescription,
                StreamThumbnail = channel.StreamThumbnail,
                ProfilePicture = channel.ProfilePicture,
                StreamColor = channel.StreamColor,
                StartTime = DateTime.UtcNow,
                Name = channel.Name
            };

            return archivedStream;
        }

        private async Task ClearChannelStreamInfo(DataModels.Channel channel)
        {
            channel.StreamTitle = null;
            channel.StreamSubject = null;
            channel.StreamDescription = null;
            channel.StreamThumbnail = null;
            channel.StreamColor = null;
            channel.ArchivedVideoId = null;
            await Save(channel.Id, channel);
        }

        private string GetCorrespondingStreamColor(string subject)
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

        private async Task DeleteFillScheduleTask(string scheduleId)
        {
            if(scheduleId != null && scheduleId != "")
                await Delete<Schedule>(scheduleId);
        }

        private int GetNumberOfVideosWithId(string id, StreamHosterRSSFeed response)
        {
            int count = 0;
            for(int i = 0; i < response.Channel.Item.Length; i++)
            {
                if (response.Channel.Item[i].StreamMediaId == id.Remove(id.Length - 2))
                {
                    count++;
                }
            }

            return count;
        }

        private List<string> GetVideoIdsWithId(string id, int videoCount, StreamHosterRSSFeed response)
        {
            List<string> ids = new List<string>();
            for (int i = 0; i < response.Channel.Item.Length; i++)
            {
                if (ids.Count == videoCount) break;

                if (response.Channel.Item[i].StreamMediaId == id.Remove(id.Length - 2))
                {
                    ids.Add(response.Channel.Item[i].MediaContentId);
                }
            }

            return ids;
        }
    }
}
