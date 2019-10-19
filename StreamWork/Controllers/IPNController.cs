using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        private HelperFunctions helperFunctions = new HelperFunctions();

        [HttpPost]
        public IActionResult Receive ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            this.storageConfig = storageConfig;

            IPNContext ipnContext = new IPNContext {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII)) {
                ipnContext.RequestBody = reader.ReadToEnd();
            }

            //Store the IPN received from PayPal
            Task.Run(() => LogRequest(ipnContext));

            //Fire and forget verification task
            Task.Run(() => VerifyTask(ipnContext));

            //Reply back a 200 code
            return Ok();
        }

        private async Task VerifyTask (IPNContext ipnContext) {
            string error = ipnContext.RequestBody;
            try {
                var verificationRequest = WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";

                //Add cmd=_notify-validate to the payload
                string strRequest = "cmd=_notify-validate&" + ipnContext.RequestBody;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII)) {
                    writer.Write(strRequest);
                }

                //Send the request to PayPal and get the response
                using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream())) {
                    ipnContext.Verification = reader.ReadToEnd();
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
            if (ipnContext.Verification.Equals("VERIFIED")) {
                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment

                await helperFunctions.SavePayment(storageConfig, new Payment {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = "EXAMPLEID0000",
                    PayerEmail = "student@example.com",
                    Student = "student",
                    Val = "100",
                    TimeSent = DateTime.Now.ToString(),
                    Verified = "TRUE",
                    Error = error
                });
            }
            else if (ipnContext.Verification.Equals("INVALID")) {
                //Log for manual investigation

                await helperFunctions.SavePayment(storageConfig, new Payment {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = "EXAMPLEID0000",
                    PayerEmail = "student@example.com",
                    Student = "student",
                    Val = "100",
                    TimeSent = DateTime.Now.ToString(),
                    Verified = "FALSE",
                    Error = "VERIFICATION_INVALID " + error
                });
            }
            else {
                //Log error

                await helperFunctions.SavePayment(storageConfig, new Payment {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = "EXAMPLEID0000",
                    PayerEmail = "student@example.com",
                    Student = "student",
                    Val = "100",
                    TimeSent = DateTime.Now.ToString(),
                    Verified = "FALSE",
                    Error = "VERIFICATION_UNKNOWN " + error
                });
            }
        }
    }
}