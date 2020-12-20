using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.MediaServices;

namespace StreamWork.Services
{
    public class StreamService : StorageService
    {
        readonly ILogger log;

        public StreamService([FromServices] IOptionsSnapshot<StorageConfig> config, ILogger logger) : base(config) { log = logger; }

        public async Task StreamStarted(string id, string action, string streamName, string category)
        {
            try
            {
                var channel = await Get<Channel>(MongoQueries.GetChannelWithChannelKey, id);

                var stream = new Stream
                {
                    Id = Guid.NewGuid().ToString(),
                    ChannelId = channel.Id,
                    Title = channel.Title,
                    Description = channel.Description
                };

                await Save(stream.Id, stream);
            }
            catch(Exception e)
            {
                log.Error("Error in StreamService -> StreamStarted " + e.Message + " : " + e.InnerException);
                Console.WriteLine("Error in StreamService -> StreamStarted " + e.Message + " : " + e.InnerException);
            }
        }

        public async Task StreamEnded(string id, string action, string streamName, string category)
        {
            
        }

        public async Task VideoReady(string id, string action, string videoName)
        {
            try
            {
                var stream = await Get<Stream>(MongoQueries.GetStreamWithChannelKey, id);

                var video = new Video
                {
                    ChannelId = stream.ChannelId,
                    Title = stream.Title,
                    Description = stream.Description,
                    ChatIds = await GetChats(stream.Id),
                };

                await Save(video.Id, video);
                await Delete<Stream>(stream.Id);
            }
            catch (Exception e)
            {
                log.Error("Error in StreamService -> VideoReady " + e.Message + " : " + e.InnerException);
                Console.WriteLine("Error in StreamService -> VideoReady " + e.Message + " : " + e.InnerException);
            }
        }

        public async Task<string[]> CreateStreamInformation(string userId) //creates stream for user, returns rtmp url and and stream id (a.k.a streamKey on OBS)
        {
            Hashtable table = new Hashtable
            {
                { "name", userId },
                { "listenerHookURL", MiscHelperMethods.streamWebHookURL }
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(table), Encoding.UTF8, "application/json");

            var response = await CallJSON<CreateStream>("https://media.streamwork.live:5443/LiveApp/rest/v2/broadcasts/create", content);
            string rtmpURL = response.RtmpUrl;
            string streamId = response.StreamId;

            return new string[] { rtmpURL, streamId };
        }

        private async Task<string[]> GetChats(string streamId)
        {
            //TODO
            return null;
        }
    }
}


