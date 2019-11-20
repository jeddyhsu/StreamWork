using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DaCastAPI;
using StreamWork.DataModels;
using StreamWork.HelperClasses;

namespace StreamWork.Threads
{
    public class ThreadClass
    {
        readonly HelperFunctions helperFunctions;
        readonly IOptionsSnapshot<StorageConfig> storageConfig;
        readonly UserChannel userChannel;
        readonly UserLogin userLogin;
        readonly string streamTitle;
        readonly string streamSubject;
        readonly string streamThumbnail;

        private int archivedVideoCount;

        public ThreadClass(IOptionsSnapshot<StorageConfig> storageConfig, UserChannel userChannel, UserLogin userLogin, string streamTitle, string streamSubject, string streamThumbnail)
        {
            helperFunctions = new HelperFunctions();
            this.userChannel = userChannel;
            this.userLogin = userLogin;
            this.storageConfig = storageConfig;
            this.streamTitle = streamTitle;
            this.streamSubject = streamSubject;
            this.streamThumbnail = streamThumbnail;
        }

        public async Task TurnRecordingOn() //Enables Stream Recording
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/" + userChannel.ChannelKey + "/recording/watch?apikey=135034_2b54d7950c64485cb8c3", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TurnRecordingOn: " + ex.Message);
            }
        }

        public async Task StartRecordingStream() //Starts Stream Recording
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/" + userChannel.ChannelKey + "/recording/start?apikey=135034_2b54d7950c64485cb8c3", null);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in StartRecordingStream: " + ex.Message);
            }
        }

        public async Task TurnRecordingOff() //Turns Recording Capability Off
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.DeleteAsync("https://api.dacast.com/v2/channel/" + userChannel.ChannelKey + "/recording/watch?apikey=135034_2b54d7950c64485cb8c3");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TurnRecordingOff: " + ex.Message);
            }
        }

        public bool RunThread()
        {
            archivedVideoCount = (int)GetArchivedVideo().TotalCount;
            bool tryAPI = true;
            //This thread handles getting streamed videos to our archive DB
            var cancellationToken = new CancellationToken();
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    userChannel.StreamSubject = streamSubject;
                    userChannel.StreamTitle = streamTitle;
                    userChannel.StreamThumbnail = streamThumbnail;
                    await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
                }
                catch(Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e.Message);
                }
                
                while (tryAPI)
                {
                    await Task.Delay(15000, cancellationToken);
                    try
                    {
                        var live = DataStore.CallAPI<LiveRecordingAPI>("https://liverecording.dacast.com/l/status/live?contentId=135034_c_" + userChannel.ChannelKey + "&apikey=135034_2b54d7950c64485cb8c3");
                        if (live.IsLive)
                            Console.WriteLine("Live");
                        else
                        {
                            Console.WriteLine("Not Live");
                            await ClearChannelStreamInfo();
                            await TurnRecordingOff();
                            RunVideoArchiveThread();
                            tryAPI = false;
                        }
                    }
                    catch(IndexOutOfRangeException e)
                    {
                        Console.WriteLine("Error in RunThread: " + e.Message);
                        tryAPI = true;
                    }
                }
            }, TaskCreationOptions.LongRunning);

            return false;
        }

        private void RunVideoArchiveThread()
        {
            var cancellationToken = new CancellationToken();
            bool archiveApi = true;
            Task.Factory.StartNew(async () =>
            {
                while (archiveApi)
                {
                    await Task.Delay(30000, cancellationToken);
                    var videoInfo = GetArchivedVideo();
                    if (videoInfo.TotalCount != archivedVideoCount)
                    {
                        Console.WriteLine("Video Ready");
                        await StopStreamAndArchive(videoInfo);
                        archiveApi = false;
                    }
                    else
                    {
                        Console.WriteLine("Video Not Ready");
                        archiveApi = true;
                    }
                       
                }
               
            }, TaskCreationOptions.LongRunning);
        }

        private async Task<bool> StopStreamAndArchive(VideoArchiveAPI archivedVideo)
        {
            UserArchivedStreams archivedStream = new UserArchivedStreams
            {
                Id = Guid.NewGuid().ToString(),
                Username = userChannel.Username,
                StreamID = archivedVideo.Data[0].Id.ToString(),
                StreamTitle = streamTitle,
                StreamSubject = streamSubject,
                StreamThumbnail = streamThumbnail,
                ProfilePicture = userLogin.ProfilePicture
            };
            try
            {
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", archivedStream.Id } }, archivedStream);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                Console.WriteLine(e.InnerException);
            }

            return true;
        }

        private VideoArchiveAPI GetArchivedVideo()
        {
            var currentDate = DateTime.Now;
            var finalDate = currentDate.ToString("ddd/MMM/d/yyyy").Replace('/', ' ');
            //strict format!!!
            var archivedVideos = DataStore.CallAPI<VideoArchiveAPI>("https://api.dacast.com/v2/vod?apikey=135034_2b54d7950c64485cb8c3&title=" + "(" + userChannel.ChannelKey + ")" + " - " + finalDate);
            return archivedVideos;
        }

        private async Task ClearChannelStreamInfo()
        {
            try
            {
                userChannel.StreamTitle = null;
                userChannel.StreamSubject = null;
                userChannel.StreamThumbnail = null;
                await DataStore.SaveAsync(helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel.Id } }, userChannel);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
