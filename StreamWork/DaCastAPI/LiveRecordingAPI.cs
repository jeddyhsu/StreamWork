using System;
using Newtonsoft.Json;

namespace StreamWork.DaCastAPI
{
    public class LiveRecordingAPI
    {
        [JsonProperty("watchingStatus")]
        public bool WatchingStatus { get; set; }

        [JsonProperty("recordingStatus")]
        public string RecordingStatus { get; set; }
    }
}
