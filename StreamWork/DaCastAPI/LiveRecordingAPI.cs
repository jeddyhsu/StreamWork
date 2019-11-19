using System;
using Newtonsoft.Json;

namespace StreamWork.DaCastAPI
{
    public class LiveRecordingAPI
    {
        [JsonProperty("isLive")]
        public bool IsLive { get; set; }
    }
}
