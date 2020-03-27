using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StreamHoster;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DaCastAPI;
using StreamWork.DataModels;
using StreamWork.HelperClasses;
using StreamWork.StreamHoster;

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

        private string _latestVideoId;

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

        public bool RunVideoThread() //This thread handles checking if the stream is still live
        {
            bool tryAPI = true;
            var cancellationToken = new CancellationToken();
            var channelIds = _userChannel.ChannelKey;
            var rssId = channelIds.Split("|")[1];

            StreamHosterRSSFeed response = (StreamHosterRSSFeed)DataStore.CallAPI<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/" + rssId + "?format=mrss");
            _latestVideoId = response.Channel.Item.Mediaid;

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
                        StreamHosterRSSFeed response = (StreamHosterRSSFeed)DataStore.CallAPI<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/" + rssId + "?format=mrss");
                        if (response.Channel.Item.Mediaid != _latestVideoId)
                        {
                            await ArchiveStream(response.Channel.Item.Mediaid);
                            await ClearChannelStreamInfo();
                            break;
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

        private async Task<bool> ArchiveStream(string streamId)
        {
            UserArchivedStreams archivedStream = new UserArchivedStreams
            {
                Id = Guid.NewGuid().ToString(),
                Username = _userChannel.Username,
                StreamID = streamId,
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
