using Newtonsoft.Json;

namespace StreamWork.StreamHoster
{
    public partial class StreamHosterEndpoint
    {
        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public StreamHosterEndpointProperties[] Data { get; set; }
    }

    public partial class StreamHosterEndpointProperties
    {
        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("cId")]
        public string CId { get; set; }

        [JsonProperty("isRecording")]
        public bool IsRecording { get; set; }
    }
}
