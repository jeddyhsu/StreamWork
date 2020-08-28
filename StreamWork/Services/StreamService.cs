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
        private static ConcurrentDictionary<string, List<Video>> streamHandler = new ConcurrentDictionary<string, List<Video>>(); //concurrent dict will provide a pipeline where we can store username associated with the videos that they are streaming (thread safe)

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

        private async Task<bool> StartRecording()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
                var resp = await client.PostAsync("https://a.streamhoster.com/v1/papi/media/stream/" + channel.ChannelKey.Remove(channel.ChannelKey.Length - 2) + "/record", new StringContent("{\"recordMode\":\"manual_on\"}", Encoding.UTF8, "application/json"));
                var response = await resp.Content.ReadAsByteArrayAsync();
                return true;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Error in IsLive: " + ex.Message);
                return false;
            }
        }

        private async Task<bool> StopRecording()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
                var resp = await client.PostAsync("https://a.streamhoster.com/v1/papi/media/stream/" + channel.ChannelKey.Remove(channel.ChannelKey.Length - 2) + "/record", new StringContent("{\"recordMode\":\"manual_off\"}", Encoding.UTF8, "application/json"));
                var response = await resp.Content.ReadAsByteArrayAsync();
                return true;
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

            Task.Factory.StartNew(async () =>
            {
                await StartRecording();

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
                            await StopRecording();
                            await Task.Delay(15000, cancellationToken);
                            await ClearChannelStreamInfo();
                            await RunVideoThread();
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

        private async Task RunVideoThread() //This thread handles checking if the stream is still live
        {
            var archivedVideo = GetVideo();

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
                    initialCount = GetNumberOfVideosWithId(channel.ChannelKey, initialResponse); //check for initial count of the rss feed

                await Task.Factory.StartNew(async () => //spawns a new thread looking for the initial count of the feed + how many items are in the video list. The video list is retreived by the username of the current user
                {
                    while (true)
                    {
                        await Task.Delay(30000);
                        StreamHosterRSSFeed response = CallXML<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/iAahx7oSswv?format=mrss");
                        if (response.Channel.Item != null && (GetNumberOfVideosWithId(channel.ChannelKey, response) >= initialCount + streamHandler[channel.Username].Count)) // we keep polling until the feed is updated to the number of videos (the initial count & concurrent dict count)
                        {
                            Console.WriteLine("Videos Found");
                            await ArchiveVideo(response);
                            break;
                        }
                    }

                }, new CancellationToken());
            }
        }

        private async Task<bool> ArchiveVideo(StreamHosterRSSFeed response)
        {
            try
            {
                var ids = GetVideoIdsWithId(channel.ChannelKey, streamHandler[channel.Username].Count, response);
                streamHandler[channel.Username].Reverse(); //reverse list content

                for (int i = 0; i < streamHandler[channel.Username].Count; i++) //go through the response as well as the dict and associate each stream to its object
                {
                    var video = streamHandler[channel.Username][i];
                    video.StreamID = ids[i];
                    video.Views = channel.Views;
                    await Save(video.Id, video);
                }

                initialCount = 0;
                streamHandler.TryRemove(channel.Username, out _);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetArchiveStream: " + ex.Message);
            }

            return true;
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
                StartTime = DateTime.UtcNow,
                Name = profile.Name
            };

            return archivedStream;
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
