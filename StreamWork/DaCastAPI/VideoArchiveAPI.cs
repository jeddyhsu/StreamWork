namespace StreamWork.DaCastAPI
{
    using System;
    using Newtonsoft.Json;

    public partial class VideoArchiveAPI
    {
        [JsonProperty("totalCount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalCount { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("abitrate")]
        public long Abitrate { get; set; }

        [JsonProperty("acodec")]
        public string Acodec { get; set; }

        [JsonProperty("ads")]
        public object Ads { get; set; }

        [JsonProperty("associated_packages")]
        public string AssociatedPackages { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("container")]
        public string Container { get; set; }

        [JsonProperty("countries_id")]
        public long CountriesId { get; set; }

        [JsonProperty("creation_date")]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty("custom_data")]
        public object CustomData { get; set; }

        [JsonProperty("noframe_security")]
        public long NoframeSecurity { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("disk_usage")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long DiskUsage { get; set; }

        [JsonProperty("duration")]
        public DateTimeOffset Duration { get; set; }

        [JsonProperty("autoplay")]
        public bool Autoplay { get; set; }

        [JsonProperty("enable_coupon")]
        public bool EnableCoupon { get; set; }

        [JsonProperty("online")]
        public bool Online { get; set; }

        [JsonProperty("enable_payperview")]
        public bool EnablePayperview { get; set; }

        [JsonProperty("publish_on_dacast")]
        public bool PublishOnDacast { get; set; }

        [JsonProperty("enable_subscription")]
        public bool EnableSubscription { get; set; }

        [JsonProperty("external_video_page")]
        public string ExternalVideoPage { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("filesize")]
        public long Filesize { get; set; }

        [JsonProperty("google_analytics")]
        public long GoogleAnalytics { get; set; }

        [JsonProperty("group_id")]
        public long GroupId { get; set; }

        [JsonProperty("hds")]
        public Uri Hds { get; set; }

        [JsonProperty("hls")]
        public string Hls { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_secured")]
        public bool IsSecured { get; set; }

        [JsonProperty("original_id")]
        public long OriginalId { get; set; }

        [JsonProperty("password")]
        public object Password { get; set; }

        [JsonProperty("pictures")]
        public Pictures Pictures { get; set; }

        [JsonProperty("player_height")]
        public long PlayerHeight { get; set; }

        [JsonProperty("player_width")]
        public long PlayerWidth { get; set; }

        [JsonProperty("player_size_id")]
        public long PlayerSizeId { get; set; }

        [JsonProperty("referers_id")]
        public long ReferersId { get; set; }

        [JsonProperty("save_date")]
        public DateTimeOffset SaveDate { get; set; }

        [JsonProperty("share_code")]
        public ShareCode ShareCode { get; set; }

        [JsonProperty("splashscreen_id")]
        public long SplashscreenId { get; set; }

        [JsonProperty("streamable")]
        public long Streamable { get; set; }

        [JsonProperty("subtitles")]
        public object Subtitles { get; set; }

        [JsonProperty("template_id")]
        public long TemplateId { get; set; }

        [JsonProperty("theme_id")]
        public long ThemeId { get; set; }

        [JsonProperty("thumbnail_id")]
        public long ThumbnailId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("vbitrate")]
        public long Vbitrate { get; set; }

        [JsonProperty("vcodec")]
        public string Vcodec { get; set; }

        [JsonProperty("video_height")]
        public long VideoHeight { get; set; }

        [JsonProperty("video_width")]
        public long VideoWidth { get; set; }
    }

    public partial class Paging
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }
    }
}
