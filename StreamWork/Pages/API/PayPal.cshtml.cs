using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.API
{
    public class PayPalModel : PageModel
    {
        class PayPalToken
        {
            [JsonProperty("scope")] public string Scope { get; set; }
            [JsonProperty("access_token")] public string AccessToken { get; set; }
            [JsonProperty("token_type")] public string TokenType { get; set; }
            [JsonProperty("app_id")] public string AppId { get; set; }
            [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
            [JsonProperty("nonce")] public string Nonce { get; set; }
        }

        private readonly StorageService storage;

        // SANDBOX
        private static readonly string clientId = "AdpoCnFrw9DdiwRavCHCjgLagktdPwClFf-UHhdkTbOXSVZSCR3Nn3lq6xSA30N_GZorSw54LwdwEM9e";
        private static readonly string clientSecret = "EFMyH5lMKflyKhnkatRDIwo8t8CR-BWKX881AK5DVJCfUrJihjErMi2JvfsQQs2f3of5lesyoAXPpJbG";

        // LIVE
        //private static readonly string clientId = "AQOvtmDCHt580quuvSACJOW0E2hVia56OWIbs2miKVF188BMmRYdxzeAwW2acHyH42C2BhNLw35FeEAd";
        //private static readonly string clientSecret = "EEEzt4_c_IHowB-CrpTWpHcnienIZ8fQ64BSXB5gErBapyVFZEm2OHcVdUy6lgaozDsydtdLABuhsp5W";

        private static readonly string scope = "";
        private static readonly string url = "";

        public PayPalModel (StorageService storage)
        {
            this.storage = storage;
        }

        // This isn't currently necessary!
        // I wrote it thinking it was, but I'm keeping it because it could be useful in the future to automate some things
        // This function has never been in production and is utterly untested
        private static async Task<PayPalToken> GetPayPalToken(string scope)
        {
            HttpResponseMessage response = await new HttpClient().PostAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>
                { { "grant_type", "client_credentials" }, { "scope", scope },
                { "client_id", clientId }, { "client_secret", clientSecret } }));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<PayPalToken>(await response.Content.ReadAsStringAsync());
        }

        public async Task OnGetWebhookAsync()
        {
            string id = Guid.NewGuid().ToString();
            await storage.Save(id, new Debug
            {
                Id = id,
                Timestamp = DateTime.UtcNow,
                Message = "Webhook received in OnGetWebhook"
            });
        }

        public async Task OnPostWebhookAsync()
        {
            string id = Guid.NewGuid().ToString();
            await storage.Save(id, new Debug
            {
                Id = id,
                Timestamp = DateTime.UtcNow,
                Message = "Webhook received in OnPostWebhook"
            });
        }
    }
}
