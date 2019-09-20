using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DaCastAPI;
using StreamWork.DataModels;
using StreamWork.Models;

namespace StreamWork.Threads
{
    public class ThreadClass
    {
        HelperFunctions helperFunctions;
        IOptionsSnapshot<StorageConfig> storageConfig;
        UserChannel userChannel;
        string streamTitle;
        string streamSubject;
        string streamThumbnail;

        public ThreadClass(IOptionsSnapshot<StorageConfig> storageConfig, UserChannel userChannel, string streamTitle, string streamSubject, string streamThumbnail)
        {
            helperFunctions = new HelperFunctions();
            this.userChannel = userChannel;
            this.storageConfig = storageConfig;
            this.streamTitle = streamTitle;
            this.streamSubject = streamSubject;
            this.streamThumbnail = streamThumbnail;
        }

        public bool RunThread()
        {
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

                bool x = true;
                while (x)
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
                            x = false;
                        }
                    }
                    catch(System.IndexOutOfRangeException e)
                    {
                        x = true;
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
                StreamThumbnail = streamThumbnail
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
