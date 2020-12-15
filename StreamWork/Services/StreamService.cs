using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using StreamWork.Config;
using StreamWork.HelperMethods;
using StreamWork.MediaServices;

namespace StreamWork.Services
{
    public class StreamService : StorageService
    {
        ILogger log;

        public StreamService([FromServices] IOptionsSnapshot<StorageConfig> config, ILogger logger) : base(config) { log = logger; }

        public async Task StreamStarted(string id, string action, string streamName, string category)
        {
            
        }

        public async Task StreamEnded(string id, string action, string streamName, string category)
        {
            //TODO
        }

        public async Task VideoReady(string id, string action, string videoName)
        {
            //TODO  
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
    }
}


