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
        readonly HomeHelperFunctions _helperFunctions;
        readonly IOptionsSnapshot<StorageConfig> _storageConfig;
        readonly UserChannel _userChannel;
        readonly UserLogin _userLogin;
        readonly string _streamTitle;
        readonly string _streamSubject;
        readonly string _streamThumbnail;

        private int _archivedVideoCount;

        public ThreadClass(IOptionsSnapshot<StorageConfig> storageConfig, UserChannel userChannel, UserLogin userLogin, string streamTitle, string streamSubject, string streamThumbnail)
        {
            _helperFunctions = new HomeHelperFunctions();
            _userChannel = userChannel;
            _userLogin = userLogin;
            _storageConfig = storageConfig;
            _streamTitle = streamTitle;
            _streamSubject = streamSubject;
            _streamThumbnail = streamThumbnail;
        }

        public async Task TurnRecordingOn() //Turns Recording Capability On
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/"
                                                           + _userChannel.ChannelKey
                                                           + "/recording/watch?apikey="
                                                           + _helperFunctions._dacastAPIKey, null);
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
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/"
                                                           + _userChannel.ChannelKey
                                                           + "/recording/start?apikey="
                                                           + _helperFunctions._dacastAPIKey, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in StartRecordingStream: " + ex.Message);
            }
        }

        public async Task StopRecordingStream() //Stops Stream Recording
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://api.dacast.com/v2/channel/"
                                                           + _userChannel.ChannelKey
                                                           + "/recording/stop?apikey="
                                                           + _helperFunctions._dacastAPIKey, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in StopRecordingStream: " + ex.Message);
            }
        }

        public async Task TurnRecordingOff() //Turns Recording Capability Off
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.DeleteAsync("https://api.dacast.com/v2/channel/"
                                                             + _userChannel.ChannelKey
                                                             + "/recording/watch?apikey="
                                                             + _helperFunctions._dacastAPIKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TurnRecordingOff: " + ex.Message);
            }
        }

        public bool RunVideoThread() //This thread handles checking if the stream is still live
        {
            _archivedVideoCount = (int)GetArchivedVideo().TotalCount;
            bool tryAPI = true;
            var cancellationToken = new CancellationToken();
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    _userChannel.StreamSubject = _streamSubject;
                    _userChannel.StreamTitle = _streamTitle;
                    _userChannel.StreamThumbnail = _streamThumbnail;
                    await DataStore.SaveAsync(_helperFunctions._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", _userChannel.Id } }, _userChannel);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e.Message);
                }

                while (tryAPI)
                {
                    await Task.Delay(3000, cancellationToken);
                    try
                    {
                        var live = DataStore.CallAPI<LiveRecordingAPI>("https://liverecording.dacast.com/l/status/live?contentId=135034_c_"
                                                                        + _userChannel.ChannelKey
                                                                        + "&apikey="
                                                                        + _helperFunctions._dacastAPIKey);
                        if (live.IsLive)
                            Console.WriteLine("Live");
                        else
                        {
                            Console.WriteLine("Not Live");
                            await ClearChannelStreamInfo();
                            await StopRecordingStream();
                            await TurnRecordingOff();
                            RunVideoArchiveThread();
                            tryAPI = false;
                        }
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        Console.WriteLine("Error in RunThread: " + ex.Message);
                        tryAPI = true;
                    }
                }
            }, TaskCreationOptions.LongRunning);

            return false;
        }

        private void RunVideoArchiveThread() //This thread handles getting streamed videos to our archive DB
        {
            var cancellationToken = new CancellationToken();
            bool archiveApi = true;
            Task.Factory.StartNew(async () =>
            {
                while (archiveApi)
                {
                    await Task.Delay(15000, cancellationToken);
                    var videoInfo = GetArchivedVideo();
                    if (videoInfo.TotalCount != _archivedVideoCount)
                    {
                        Console.WriteLine("Video Ready");
                        await StopStreamAndArchive(videoInfo);
                        archiveApi = false;
                        break;
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
                Username = _userChannel.Username,
                StreamID = archivedVideo.Data[0].Id.ToString(),
                StreamTitle = _streamTitle,
                StreamSubject = _streamSubject,
                StreamThumbnail = _streamThumbnail,
                ProfilePicture = _userLogin.ProfilePicture
            };
            try
            {
                await DataStore.SaveAsync(_helperFunctions._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", archivedStream.Id } }, archivedStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in StopStreamAndArchive " + ex.Message);
            }

            return true;
        }

        private VideoArchiveAPI GetArchivedVideo()
        {
            var currentDate = DateTime.Now;
            var finalDate = currentDate.AddHours(GetHoursAheadBasedOnTimeZone()).ToString("ddd/MMM/dd/yyyy").Replace('/', ' ');
            var archivedVideos = DataStore.CallAPI<VideoArchiveAPI>("https://api.dacast.com/v2/vod?apikey="
                                                                     + _helperFunctions._dacastAPIKey
                                                                     + "&title=" + "Live recording ("
                                                                     + _userChannel.ChannelKey
                                                                     + ")"
                                                                     + " - " + finalDate);   //strict format!!!
            return archivedVideos;
        }

        private int GetHoursAheadBasedOnTimeZone()
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            switch (localZone.DisplayName)
            {
                case "GMT-08:00": //PST
                    return 7;
                case "GMT-07:00": //MST
                    return 7;
            }

            return 0;
        }

        private async Task ClearChannelStreamInfo()
        {
            try
            {
                _userChannel.StreamTitle = null;
                _userChannel.StreamSubject = null;
                _userChannel.StreamThumbnail = null;
                await DataStore.SaveAsync(_helperFunctions._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", _userChannel.Id } }, _userChannel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ClearChannelStreamInfo " + ex.Message);
            }
        }
    }
}
