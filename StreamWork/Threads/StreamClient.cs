using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StreamHoster;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperClasses;

namespace StreamWork.Threads
{
    public class StreamClient
    {
        readonly HomeMethods _homeHelperFunctions;
        readonly EmailMethods _emailHelperFunctions;
        readonly StreamClientMethods _threadClassHelperFunctions;
        readonly ChatMethods _chatHelperFunctions;
        readonly IOptionsSnapshot<StorageConfig> _storageConfig;
        readonly UserChannel _userChannel;
        readonly UserLogin _userLogin;
        readonly string _streamTitle;
        readonly string _streamSubject;
        readonly string _streamDescription;
        readonly string _streamThumbnail;
        readonly string _archivedVideoId;
        readonly string _chatColor;

        private int initialCount = 0;
        private static int threadCount = 0;
        private static Hashtable hashTable = new Hashtable();

        public StreamClient(IOptionsSnapshot<StorageConfig> storageConfig, UserChannel userChannel, UserLogin userLogin, string streamTitle, string streamSubject, string streamDescription, string streamThumbnail, string archivedVideoId, string chatColor)
        {
            _homeHelperFunctions = new HomeMethods();
            _emailHelperFunctions = new EmailMethods();
            _threadClassHelperFunctions = new StreamClientMethods();
            _chatHelperFunctions = new ChatMethods();
            _storageConfig = storageConfig;
            _userChannel = userChannel;
            _userLogin = userLogin;
            _streamTitle = streamTitle;
            _streamSubject = streamSubject;
            _streamDescription = streamDescription;
            _streamThumbnail = streamThumbnail;
            _archivedVideoId = archivedVideoId;
            _chatColor = chatColor;
        }

        public bool RunEmailThread()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _emailHelperFunctions.SendOutMassEmail(_storageConfig, _userLogin, _userChannel, _archivedVideoId);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e.Message);
                }

            }, TaskCreationOptions.LongRunning);

            return true;
        }

        public bool RunLiveThread()
        {
            bool tryAPI = true;
            var cancellationToken = new CancellationToken();
            var channelIds = _userChannel.ChannelKey;
            var channelKey = channelIds.Split("|")[0];

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    _userChannel.StreamSubject = _streamSubject;
                    _userChannel.StreamTitle = _streamTitle;
                    _userChannel.StreamDescription = _streamDescription;
                    _userChannel.StreamThumbnail = _streamThumbnail;
                    _userChannel.Views = 0;
                    _userChannel.ChatColor = _chatColor;
                    await DataStore.SaveAsync(_homeHelperFunctions._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", _userChannel.Id } }, _userChannel);
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
                        var response = _threadClassHelperFunctions.CheckIfUserChannelIsLive(_userChannel.ChannelKey);
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

        public void RunVideoThread() //This thread handles checking if the stream is still live
        {
            bool tryAPI = true;
            var cancellationToken = new CancellationToken();
            var channelIds = _userChannel.ChannelKey;
            var rssId = channelIds.Split("|")[1];

            var archivedVideo = GetArchiveStream();
            threadCount += 1;
            hashTable.Add(threadCount, archivedVideo);

            StreamHosterRSSFeed initialResponse = (StreamHosterRSSFeed)DataStore.CallAPI<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/" + rssId + "?format=mrss");
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
                            StreamHosterRSSFeed response = (StreamHosterRSSFeed)DataStore.CallAPI<StreamHosterRSSFeed>("https://c.streamhoster.com/feed/WxsdDM/mAe0epZsixC/" + rssId + "?format=mrss");
                            if (response.Channel.Item != null && response.Channel.Item.Length == initialCount + threadCount)
                            {
                                Console.WriteLine("Videos Found");
                                await ArchiveStreams(response);
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

        private UserArchivedStreams GetArchiveStream()
        {
            UserArchivedStreams archivedStream = new UserArchivedStreams
            {
                Id = _archivedVideoId,
                Username = _userChannel.Username,
                StreamID = "",
                StreamTitle = _streamTitle,
                StreamSubject = _streamSubject,
                StreamDescription = _streamDescription,
                StreamThumbnail = _streamThumbnail,
                ProfilePicture = _userLogin.ProfilePicture
            };

            return archivedStream;
        }

        private async Task<bool> ArchiveStreams(StreamHosterRSSFeed response) // HI SELF DO THIS
        {
            for (int i = 1; i < hashTable.Count + 1; i++)
            {
                var archivedStream = (UserArchivedStreams)hashTable[i];
                archivedStream.StreamID = response.Channel.Item[threadCount - i].Mediaid;
                archivedStream.Views = _userChannel.Views;
                archivedStream.StartTime = DateTime.UtcNow;

                try
                {
                    await DataStore.SaveAsync(_homeHelperFunctions._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", archivedStream.Id } }, archivedStream);
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
                _userChannel.StreamTitle = null;
                _userChannel.StreamSubject = null;
                _userChannel.StreamDescription = null;
                _userChannel.StreamThumbnail = null;
                await _chatHelperFunctions.DeleteAllChatsWithChatId(_storageConfig, _userChannel.Username);
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, _storageConfig.Value, new Dictionary<string, object> { { "Id", _userChannel.Id } }, _userChannel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ClearChannelStreamInfo " + ex.Message);
            }
        }
    }
}
