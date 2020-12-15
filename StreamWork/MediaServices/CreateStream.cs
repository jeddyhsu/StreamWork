using System;
using Newtonsoft.Json;

namespace StreamWork.MediaServices
{
    public partial class CreateStream
    {
        [JsonProperty("streamId")]
        public string StreamId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("publish")]
        public bool Publish { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("plannedStartDate")]
        public long PlannedStartDate { get; set; }

        [JsonProperty("plannedEndDate")]
        public long PlannedEndDate { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("endPointList")]
        public object EndPointList { get; set; }

        [JsonProperty("publicStream")]
        public bool PublicStream { get; set; }

        [JsonProperty("is360")]
        public bool Is360 { get; set; }

        [JsonProperty("listenerHookURL")]
        public Uri ListenerHookUrl { get; set; }

        [JsonProperty("category")]
        public object Category { get; set; }

        [JsonProperty("ipAddr")]
        public object IpAddr { get; set; }

        [JsonProperty("username")]
        public object Username { get; set; }

        [JsonProperty("password")]
        public object Password { get; set; }

        [JsonProperty("quality")]
        public object Quality { get; set; }

        [JsonProperty("speed")]
        public long Speed { get; set; }

        [JsonProperty("streamUrl")]
        public object StreamUrl { get; set; }

        [JsonProperty("originAdress")]
        public string OriginAdress { get; set; }

        [JsonProperty("mp4Enabled")]
        public long Mp4Enabled { get; set; }

        [JsonProperty("webMEnabled")]
        public long WebMEnabled { get; set; }

        [JsonProperty("expireDurationMS")]
        public long ExpireDurationMs { get; set; }

        [JsonProperty("rtmpURL")]
        public string RtmpUrl { get; set; }

        [JsonProperty("zombi")]
        public bool Zombi { get; set; }

        [JsonProperty("pendingPacketSize")]
        public long PendingPacketSize { get; set; }

        [JsonProperty("hlsViewerCount")]
        public long HlsViewerCount { get; set; }

        [JsonProperty("webRTCViewerCount")]
        public long WebRtcViewerCount { get; set; }

        [JsonProperty("rtmpViewerCount")]
        public long RtmpViewerCount { get; set; }

        [JsonProperty("startTime")]
        public long StartTime { get; set; }

        [JsonProperty("receivedBytes")]
        public long ReceivedBytes { get; set; }

        [JsonProperty("bitrate")]
        public long Bitrate { get; set; }

        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        [JsonProperty("latitude")]
        public object Latitude { get; set; }

        [JsonProperty("longitude")]
        public object Longitude { get; set; }

        [JsonProperty("altitude")]
        public object Altitude { get; set; }

        [JsonProperty("mainTrackStreamId")]
        public object MainTrackStreamId { get; set; }

        [JsonProperty("subTrackStreamIds")]
        public object SubTrackStreamIds { get; set; }

        [JsonProperty("absoluteStartTimeMs")]
        public long AbsoluteStartTimeMs { get; set; }

        [JsonProperty("webRTCViewerLimit")]
        public long WebRtcViewerLimit { get; set; }

        [JsonProperty("hlsViewerLimit")]
        public long HlsViewerLimit { get; set; }
    }

}
