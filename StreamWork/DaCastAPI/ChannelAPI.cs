namespace StreamWork.DaCastAPI
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ChannelAPI
    {
        [JsonProperty("ads")]
        public object Ads { get; set; }

        [JsonProperty("associated_packages")]
        public string AssociatedPackages { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("company_url")]
        public string CompanyUrl { get; set; }

        [JsonProperty("countdown_date")]
        public object CountdownDate { get; set; }

        [JsonProperty("countdown_timezone")]
        public object CountdownTimezone { get; set; }

        [JsonProperty("counter_live_limit")]
        public long CounterLiveLimit { get; set; }

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

        [JsonProperty("google_analytics")]
        public long GoogleAnalytics { get; set; }

        [JsonProperty("hds")]
        public object Hds { get; set; }

        [JsonProperty("hls")]
        public object Hls { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_secure")]
        public object IsSecure { get; set; }

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

        [JsonProperty("config")]
        public Config Config { get; set; }

        [JsonProperty("referers_id")]
        public long ReferersId { get; set; }

        [JsonProperty("rtmp")]
        public string Rtmp { get; set; }

        [JsonProperty("save_date")]
        public DateTimeOffset SaveDate { get; set; }

        [JsonProperty("schedule")]
        public object Schedule { get; set; }

        [JsonProperty("share_code")]
        public ShareCode ShareCode { get; set; }

        [JsonProperty("splashscreen_id")]
        public long SplashscreenId { get; set; }

        [JsonProperty("stream_tech")]
        public string StreamTech { get; set; }

        [JsonProperty("theme_id")]
        public long ThemeId { get; set; }

        [JsonProperty("thumbnail_id")]
        public long ThumbnailId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("web_dvr")]
        public long WebDvr { get; set; }
    }

    public partial class Config
    {
        [JsonProperty("publishing_point_type")]
        public object PublishingPointType { get; set; }

        [JsonProperty("publishing_point_primary")]
        public string PublishingPointPrimary { get; set; }

        [JsonProperty("publishing_point_backup")]
        public string PublishingPointBackup { get; set; }

        [JsonProperty("stream_name")]
        public string StreamName { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Password { get; set; }

        [JsonProperty("live_transcoding")]
        public bool LiveTranscoding { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }
    }

    public partial class Pictures
    {
        [JsonProperty("thumbnail")]
        public object[] Thumbnail { get; set; }

        [JsonProperty("splashscreen")]
        public object[] Splashscreen { get; set; }
    }

    public partial class ShareCode
    {
        [JsonProperty("facebook")]
        public Uri Facebook { get; set; }

        [JsonProperty("twitter")]
        public Uri Twitter { get; set; }

        [JsonProperty("gplus")]
        public Uri Gplus { get; set; }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
