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
            LogRequest(ipnContext);

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


        private void LogRequest (IPNContext ipnContext) {
            // Persist the request values into a database or temporary data store
        }

        private async Task ProcessVerificationResponse (IPNContext ipnContext, string error) {
            if (ipnContext.Verification.Equals("VERIFIED")) {
                await helperFunctions.SaveDonation(storageConfig, new Donation {
                    Id = Guid.NewGuid().ToString(),
                    Student = "student",
                    Tutor = "streamtutor",
                    Val = "100",
                    TimeSent = DateTime.Now.ToString(),
                    Verified = "TRUE",
                    Error = error + "...Verification VERIFIED"
                });
                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment
            }
            else if (ipnContext.Verification.Equals("INVALID")) {
                await helperFunctions.SaveDonation(storageConfig, new Donation {
                    Id = Guid.NewGuid().ToString(),
                    Student = "student",
                    Tutor = "streamtutor",
                    Val = "100",
                    TimeSent = DateTime.Now.ToString(),
                    Verified = "FALSE",
                    Error = error + "...Verification INVALID..." + ipnContext.RequestBody
                });
                //Log for manual investigation
            }
            else {
                await helperFunctions.SaveDonation(storageConfig, new Donation {
                    Id = Guid.NewGuid().ToString(),
                    Student = "student",
                    Tutor = "streamtutor",
                    Val = "100",
                    TimeSent = DateTime.Now.ToString(),
                    Error = error + "...An unknown error occured with verification"
                });
                //Log error
            }
        }
    }
}
/*
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Core;
using StreamWork.Config;
using StreamWork.Models;

namespace StreamWork.Controllers { // IPN = Instant Payment Notification
    public class IPNController : Controller {

        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private IOptionsSnapshot<StorageConfig> storageConfig;

        HelperFunctions helperFunctions = new HelperFunctions();

        private class IPNContext {
            public HttpRequest IPNRequest { get; set; }
            public string RequestBody { get; set; }
            public string Verification { get; set; } = string.Empty;
        }

        public IActionResult IPN ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig) {
            return Receive(storageConfig);
        }

        // Should run when an IPN is recieved
        [HttpPost]
        public IActionResult Receive (IOptionsSnapshot<StorageConfig> _storageConfig) {
            storageConfig = _storageConfig;

            IPNContext ipnContext = new IPNContext {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII)) {
                ipnContext.RequestBody = reader.ReadToEnd();
            }

            //Store the IPN received from PayPal
            Donation donation = LogRequest(ipnContext);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(ipnContext, donation));

            //Reply back a 200 code
            return Ok();
        }

        private async Task VerifyTask (IPNContext ipnContext, Donation donation) {
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
                //Capture exception for manual investigation
                donation.Error += "...Failed to verify IPN task: " + exception.Message;
            }

            await ProcessVerificationResponse(ipnContext, donation);
        }

        // Should find the table entry created when payment was made, set verified to FALSE, and return it, and optionally discard incomplete donations.
        private Donation LogRequest (IPNContext ipnContext) {
            // Persist the request values into a database or temporary data store

            return new Donation {
                Id = Guid.NewGuid().ToString(), // Generated earlier
                Student = "student123", // Earlier
                Tutor = "streamtutor123", // Earlier
                Val = "20.00",
                TimeSent = DateTime.Now.ToString(), // Earlier // NOTE: timestamp is 7 hours ahead of us
                Verified = "FALSE",
                Error = ""
            };
        }

        // Should update table entry on payment validity
        private async Task ProcessVerificationResponse (IPNContext ipnContext, Donation donation) {
            if (ipnContext.Verification.Equals("VERIFIED")) {
                if (donation.Error.Equals("")) {
                    donation.Verified = "TRUE";
                }

                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment
            } else if (ipnContext.Verification.Equals("INVALID")) {
                donation.Error += "...IPN could not be verified because it was found to be INVALID";
            } else {
                donation.Error += "...IPN could not be verified for unknown reasons (neither VALID nor INVALID)";
            }

            donation.Error += "...DEBUG NOTES...IPNRequest.Body = " + ipnContext.IPNRequest.Body + "...RequestBody = " + ipnContext.RequestBody + "...Verification = " + ipnContext.Verification;

            await helperFunctions.SaveDonation(storageConfig, donation);
        }
    }
}
*/