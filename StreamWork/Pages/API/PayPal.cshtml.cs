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
    [IgnoreAntiforgeryToken]
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

        //private static readonly string scope = "";
        private static readonly string url = "";

        public PayPalModel (StorageService storage)
        {
            this.storage = storage;
        }

        // This isn't currently necessary!
        // I wrote it thinking it was, but I'm keeping it because it could be useful in the future to automate some things
        // This function has never been used in production and is utterly untested
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

        public class PayPalAmount
        {
            [JsonProperty("total")] public string Total { get; set; }
            [JsonProperty("currency")] public string Currency { get; set; }
        }

        public class PayPalTransactionFee
        {
            [JsonProperty("value")] public string Value { get; set; }
            [JsonProperty("currency")] public string Currency { get; set; }
        }

        public class PayPalResourceLink
        {
            [JsonProperty("href")] public string Href { get; set; }
            [JsonProperty("rel")] public string Rel { get; set; }
            [JsonProperty("method")] public string Method { get; set; }
        }

        public class PayPalLink
        {
            [JsonProperty("href")] public string Href { get; set; }
            [JsonProperty("rel")] public string Rel { get; set; }
            [JsonProperty("method")] public string Method { get; set; }
            [JsonProperty("encType")] public string EncType { get; set; } // Yes, specifically this is camelCase
        }

        public class PayPalResource
        {
            [JsonProperty("parent_parment")] public string Parent_Payment { get; set; }
            [JsonProperty("update_time")] public string Update_Time { get; set; }
            [JsonProperty("amount")] public PayPalAmount Amount { get; set; }
            [JsonProperty("is_final_capture")] public bool Is_Final_Capture { get; set; }
            [JsonProperty("create_time")] public string Create_Time { get; set; } // Should be DateTime?
            [JsonProperty("transaction_fee")] public PayPalTransactionFee Transaction_Fee { get; set; }
            [JsonProperty("links")] public PayPalResourceLink[] Links { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("state")] public string State { get; set; }
            [JsonProperty("reasonCode")] public string ReasonCode { get; set; } // Yes, specifically this is camelCase
        }

        public class PayPalWebhook
        {
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("create_time")] public string Create_Time { get; set; } // Should be DateTime?
            [JsonProperty("resource_type")] public string Resource_Type { get; set; }
            [JsonProperty("event_type")] public string Event_Type { get; set; }
            [JsonProperty("summary")] public string Summary { get; set; }
            [JsonProperty("resource")] public PayPalResource Resource { get; set; }
            [JsonProperty("links")] public PayPalLink[] Links { get; set; }
            [JsonProperty("event_version")] public string Event_Version { get; set; }
        }

        // Don't point webhooks to this method on multiple sessions of the site at the same time!!!
        // Ex. If webhooks go to the main site, webhooks must not go to the test site
        // This is to make sure that payments are only processed once!
        // If ever necessary, add logic to determine whether it's the test site, and process accordingly
        public async Task<IActionResult> OnPost(PayPalWebhook webhook)
        {
            string id = Guid.NewGuid().ToString();
            await storage.Save(id, new Debug
            {
                Id = id,
                Timestamp = DateTime.UtcNow,
                Message = webhook.Event_Type
            });

            return null;
        }
    }
}
