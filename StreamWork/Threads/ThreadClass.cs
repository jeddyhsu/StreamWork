using System;
using System.Collections.Generic;
using System.Net;
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
        HelperFunctions helperFunctions;
        IOptionsSnapshot<StorageConfig> storageConfig;
        UserChannel userChannel;
        UserLogin userLogin;
        string streamTitle;
        string streamSubject;
        string streamThumbnail;

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

        public async Task StartRecordingStream()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/505911/recording/start?apikey=135034_9d5e445816dfcd2a96ad",null);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in StartRecordingStream: " + ex.Message);
            }
        }

        public async Task StopRecordingStream()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/505911/recording/stop?apikey=135034_9d5e445816dfcd2a96ad", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in StopRecordingStream: " + ex.Message);
            }
        }

        public bool RunThread()
        {
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
                    await Task.Delay(60000, cancellationToken);
                    try
                    {
                        var liveRecording = DataStore.CallAPI<LiveRecordingAPI>("https://api.dacast.com/v2/channel/" + userChannel.ChannelKey + "/recording/watch?apikey=135034_9d5e445816dfcd2a96ad&_format=JSON");
                        if (liveRecording.RecordingStatus == "recording")
                        {
                            Console.WriteLine("Recording");
                        }
                        else
                        {
                            await StopStreamAndArchive();
                            await ClearChannelStreamInfo();
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

        private async Task<bool> StopStreamAndArchive()
        {
            //stop stream and archvie video into database
            var currentDate = DateTime.Now.ToString("ddd/MMM/d/yyyy").Replace('/', ' ');

            //strict format!!!
            var archivedVideo = DataStore.CallAPI<VideoArchiveAPI>("https://api.dacast.com/v2/vod?apikey=135034_9d5e445816dfcd2a96ad&title=" + "(" + userChannel.ChannelKey + ")" + " - " + currentDate);

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
