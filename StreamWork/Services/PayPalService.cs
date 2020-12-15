// See https://github.com/paypal/PayPal-NET-SDK/blob/develop/Samples/Source/WebhookGetAll.aspx.cs

using System;
using System.Threading.Tasks;
using PayPal.Api;
using StreamWork.DataModels;

namespace StreamWork.Services {
    public class PayPalService {
        private readonly StorageService storage;

        private static DateTime lastCheck = DateTime.UtcNow;

        // SANDBOX
        private static readonly string clientId = "AdpoCnFrw9DdiwRavCHCjgLagktdPwClFf-UHhdkTbOXSVZSCR3Nn3lq6xSA30N_GZorSw54LwdwEM9e";
        private static readonly string clientSecret = "EFMyH5lMKflyKhnkatRDIwo8t8CR-BWKX881AK5DVJCfUrJihjErMi2JvfsQQs2f3of5lesyoAXPpJbG";

        // LIVE
        //private static readonly string clientId = "AQOvtmDCHt580quuvSACJOW0E2hVia56OWIbs2miKVF188BMmRYdxzeAwW2acHyH42C2BhNLw35FeEAd";
        //private static readonly string clientSecret = "EEEzt4_c_IHowB-CrpTWpHcnienIZ8fQ64BSXB5gErBapyVFZEm2OHcVdUy6lgaozDsydtdLABuhsp5W";

        public PayPalService(StorageService storage) => this.storage = storage;

        public async Task ProcessNewWebhooks () {
            DateTime earliest = lastCheck;
            lastCheck = DateTime.UtcNow;
            DateTime latest = lastCheck;

            foreach (Payment payment in Payment.List(new APIContext(new OAuthTokenCredential(clientId, clientSecret).GetAccessToken())).payments) {
                DateTime createTime = DateTime.Parse(payment.create_time, null, System.Globalization.DateTimeStyles.RoundtripKind);
                if (earliest < createTime && createTime < latest) {
                    foreach (Transaction transaction in payment.transactions) {
                        Profile tutor = await storage.Get<Profile>(HelperMethods.SQLQueries.GetUserWithUsername, transaction.custom.Split('&')[1]);
                        tutor.Balance += decimal.Parse(transaction.amount.total);
                        await storage.Save(tutor.Id, tutor);
                    }
                }
            }
        }
    }
}
