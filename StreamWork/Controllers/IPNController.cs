using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Models;
using StreamWork.HelperClasses;

namespace StreamWork.Controllers {
    public class IPNController : Controller {
        private class IPNContext {
            public HttpRequest IPNRequest { get; set; }

            public string RequestBody { get; set; }

            public string Verification { get; set; } = string.Empty;
        }

        private IOptionsSnapshot<StorageConfig> storageConfig;
        private readonly HelperFunctions helperFunctions = new HelperFunctions();

        [HttpPost]
        public IActionResult Receive ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            this.storageConfig = storageConfig;

            IPNContext ipnContext = new IPNContext {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII)) {
                ipnContext.RequestBody = reader.ReadToEnd();
                reader.Close();
            }

            //Store the IPN received from PayPal
            Task.Run(() => LogRequest(new IPNContext {
                IPNRequest = ipnContext.IPNRequest,
                RequestBody = string.Copy(ipnContext.RequestBody)
            }));

            //Fire and forget verification task
            Task.Run(() => VerifyTask(new IPNContext {
                IPNRequest = ipnContext.IPNRequest,
                RequestBody = string.Copy(ipnContext.RequestBody)
            }));

            //Reply back a 200 code
            return Ok();
        }

        private async Task VerifyTask (IPNContext ipnContext) {
            string error = string.Empty;
            try {
                var verificationRequest = WebRequest.Create("https://ipnpb.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";

                //Add cmd=_notify-validate to the payload
                string strRequest = "cmd=_notify-validate&" + ipnContext.RequestBody;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII)) {
                    writer.Write(strRequest);
                    writer.Close();
                }

                //Send the request to PayPal and get the response
                using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream())) {
                    ipnContext.Verification = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception exception) {
                error = exception.Message;
            }

            await ProcessVerificationResponse(ipnContext, error);
        }


        private async Task LogRequest (IPNContext ipnContext) {
            // Persist the request values into a database or temporary data store

            await helperFunctions.LogIPNRequest(storageConfig, new IPNRequestBody {
                Id = Guid.NewGuid().ToString(),
                RequestBody = ipnContext.RequestBody
            });
        }

        private async Task ProcessVerificationResponse (IPNContext ipnContext, string error) {
            try {
                // For test parsing
                //ipnContext.RequestBody = "mc_gross=19.95&protection_eligibility=Eligible&address_status=confirmed&payer_id=LPLWNMTBWMFAY&tax=0.00&address_street=1+Main+St&payment_date=20%3A12%3A59+Jan+13%2C+2009+PST&payment_status=Completed&charset=windows-1252&address_zip=95131&first_name=Test&mc_fee=0.88&address_country_code=US&address_name=Test+User&notify_version=2.6&custom=&payer_status=verified&address_country=United+States&address_city=San+Jose&quantity=1&verify_sign=AtkOfCXbDm2hu0ZELryHFjY-Vb7PAUvS6nMXgysbElEn9v-1XcmSoGtf&payer_email=gpmac_1231902590_per%40paypal.com&txn_id=61E67681CH3238416&payment_type=instant&last_name=User&address_state=CA&receiver_email=gpmac_1231902686_biz%40paypal.com&payment_fee=0.88&receiver_id=S8XGHLYDW9T3S&txn_type=express_checkout&item_name=&mc_currency=USD&item_number=&residence_country=US&test_ipn=1&handling_amount=0.00&transaction_subject=&payment_gross=19.95&shipping=0.00";
                //ipnContext.RequestBody = "mc_gross=15.00&protection_eligibility=Eligible&address_status=confirmed&payer_id=LPLWNMTBWMFAY&tax=0.00&address_street=1+Main+St&payment_date=20%3A12%3A59+Jan+13%2C+2009+PST&payment_status=Completed&charset=windows-1252&address_zip=95131&first_name=Tom&mc_fee=0.88&address_country_code=US&address_name=Tom+Hansen&notify_version=2.6&custom=tom~rarun321&payer_status=verified&address_country=United+States&address_city=San+Jose&quantity=1&verify_sign=AtkOfCXbDm2hu0ZELryHFjY-Vb7PAUvS6nMXgysbElEn9v-1XcmSoGtf&payer_email=THansenAZ%40icloud.com&txn_id=61E67681CH3238423&payment_type=instant&last_name=Hansen&address_state=CA&receiver_email=streamworktutor%40gmail.com&payment_fee=0.88&receiver_id=S8XGHLYDW9T3S&txn_type=express_checkout&item_name=DONATION&mc_currency=USD&item_number=&residence_country=US&test_ipn=1&handling_amount=0.00&transaction_subject=&payment_gross=15.00&shipping=0.00";
                NameValueCollection request = HttpUtility.ParseQueryString(ipnContext.RequestBody);

                if (ipnContext.RequestBody.Equals("")) {
                    await helperFunctions.SavePayment(storageConfig, new Payment {
                        Id = Guid.NewGuid().ToString(),
                        TransactionId = "UNKNOWN",
                        PayerEmail = "UNKNOWN",
                        Student = "UNKNOWN",
                        Tutor = "UNKNOWN",
                        PaymentType = "UNKNOWN",
                        Val = (decimal)0f,
                        TimeSent = DateTime.UtcNow,
                        Verified = ipnContext.Verification.Equals("VERIFIED") ? "TRUE" : "FALSE",
                        Error = "NO_DATA " + error
                    });
                }
                else if (ipnContext.Verification.Equals("VERIFIED") || "1".Equals(request["test_ipn"])) {
                    // check that Payment_status=Completed
                    // check that Txn_id has not been previously processed
                    // check that Receiver_email is your Primary PayPal email
                    // check that Payment_amount/Payment_currency are correct
                    // process payment

                    if (!request["receiver_email"].ToLower().Equals("streamworktutor@gmail.com")) {
                        error = "INCORRECT_RECEIVER " + error;
                    }

                    if (!"1".Equals(request["test_ipn"])) {
                        error = "TEST " + error;
                    }

                    if (await helperFunctions.GetPayment(storageConfig, "PaymentsById", request["txn_id"]) != null) {
                        error = "DUPLICATE " + error;
                    }

                    var customParams = request["custom"].Split("~");
                    var username = customParams[0];
                    var tutorName = customParams.Length >= 2 ? customParams[1] : null;
                    var user = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, username);
                    if (user == null) {
                        error = "UNMATCHED_USER " + error;
                    }

                    if (request["item_name"].Equals("SUBSCRIPTION")) { // User is making subscription payment
                        if (!request["mc_currency"].Equals("USD")) {
                            error = "INCORRECT_CURRENCY " + error;

                        }
                        else if (!user.TrialAccepted && !request["mc_gross"].Equals("15.00")) {
                            error = "INCORRECT_VALUE" + error;

                        }
                        else {
                            if (error.Equals("")) {
                                if (DateTime.Compare(user.Expiration, DateTime.UtcNow) < 0) { // Subscription has expired
                                    user.Expiration = DateTime.UtcNow.AddMonths(1);

                                }
                                else { // Subscription is still in effect; add to current subscription
                                    user.Expiration = user.Expiration.AddMonths(1);
                                }

                                user.TrialAccepted = true;
                                await helperFunctions.UpdateUser(storageConfig, user);
                            }
                        }

                    }
                    else if (request["item_name"].Equals("DONATION")) { // User is donating to a tutor
                        if (tutorName == null) {
                            error = "NO_TUTOR " + error;

                        }
                        else {
                            var tutors = await helperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, tutorName);
                            if (tutors.Count == 0) {
                                error = "UNMATCHED_TUTOR " + error;
                            }

                            float donationAmt = float.Parse(request["payment_gross"]) * 0.9f - float.Parse(request["payment_fee"]) - float.Parse(request["tax"]);

                            if (error.Equals("")) {
                                var tutor = tutors[0];
                                tutor.Balance += (decimal)donationAmt;
                                await helperFunctions.UpdateUser(storageConfig, tutor);
                            }
                        }
                    }
                    else {
                        error = "UNKNOWN_ITEM " + error;
                    }

                    await helperFunctions.SavePayment(storageConfig, new Payment {
                        Id = Guid.NewGuid().ToString(),
                        TransactionId = request["txn_id"],
                        PayerEmail = request["payer_email"],
                        Student = username,
                        Tutor = tutorName ?? "",
                        PaymentType = request["item_name"],
                        Val = (decimal)float.Parse(request["payment_gross"]),
                        TimeSent = DateTime.UtcNow,
                        Verified = "TRUE",
                        Error = error
                    });
                }
                else if (ipnContext.Verification.Equals("INVALID")) {
                    //Log for manual investigation

                    var customParams = request["custom"].Split("~");
                    var username = customParams[0];
                    var tutorName = customParams.Length >= 2 ? customParams[1] : null;

                    await helperFunctions.SavePayment(storageConfig, new Payment {
                        Id = Guid.NewGuid().ToString(),
                        TransactionId = request["txn_id"],
                        PayerEmail = request["payer_email"],
                        Student = username,
                        Tutor = tutorName ?? "",
                        PaymentType = request["item_name"],
                        Val = (decimal)float.Parse(request["payment_gross"]),
                        TimeSent = DateTime.UtcNow,
                        Verified = "FALSE",
                        Error = "VERIFICATION_INVALID " + error
                    });
                }
                else {
                    //Log error

                    var customParams = request["custom"].Split("~");
                    var username = customParams[0];
                    var tutorName = customParams.Length >= 2 ? customParams[1] : null;

                    await helperFunctions.SavePayment(storageConfig, new Payment {
                        Id = Guid.NewGuid().ToString(),
                        TransactionId = request["txn_id"],
                        PayerEmail = request["payer_email"],
                        Student = username,
                        Tutor = tutorName ?? "",
                        PaymentType = request["item_name"],
                        Val = (decimal)float.Parse(request["payment_gross"]),
                        TimeSent = DateTime.UtcNow,
                        Verified = "FALSE",
                        Error = "VERIFICATION_UNKNOWN " + error
                    });
                }
            }
            catch (Exception e) {
                await helperFunctions.SavePayment(storageConfig, new Payment {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = "",
                    PayerEmail = "",
                    Student = "",
                    Tutor = "",
                    PaymentType = "",
                    Val = decimal.Zero,
                    TimeSent = DateTime.UtcNow,
                    Verified = "",
                    Error = e.ToString()
                });
            }
        }
    }
}