using System;
using Newtonsoft.Json;

namespace StreamWork.Oauth
{
    public class GoogleOauth
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

    }
}
