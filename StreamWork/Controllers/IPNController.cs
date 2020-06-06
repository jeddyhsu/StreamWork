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
using StreamWork.DataModels;
using System.Collections.Generic;

namespace StreamWork.Controllers
{
    public class IPNController : Controller
    {
        private IOptionsSnapshot<StorageConfig> storageConfig;
        private readonly HomeMethods _homeHelperFunctions = new HomeMethods();
        private readonly EmailMethods _emailHelperFunctions = new EmailMethods();

        [HttpPost]
        public IActionResult WebhookB1A0139C65AC29499733B(PayPalNotification notification, [FromServices] IOptionsSnapshot<StorageConfig> _storageConfig)
        {
            storageConfig = _storageConfig;
            return VerifyPayPalNotification(notification).Result;
        }

        private async Task<IActionResult> VerifyPayPalNotification(PayPalNotification notification)
        {
            // https://ipnpb.paypal.com/cgi-bin/webscr
            // https://ipnpb.sandbox.paypal.com/cgi-bin/webscr
            var verificationRequest = WebRequest.Create("https://ipnpb.paypal.com/cgi-bin/webscr");

            verificationRequest.Method = "POST";
            verificationRequest.ContentType = "application/x-www-form-urlencoded";

            string strRequest = "cmd=_notify-validate&";
            verificationRequest.ContentLength = strRequest.Length;

            using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII))
            {
                writer.Write(strRequest);
                writer.Close();
            }

            string verification = "";
            using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream()))
            {
                verification = reader.ReadToEnd();
                reader.Close();
            }

            await ProcessPayPalNotification(notification, verification);

            return Ok();
        }

        private async Task<IActionResult> ProcessPayPalNotification(PayPalNotification notification, string verification)
        {

            try
            {
                string error = "";
                bool subscription = false;
                UserLogin student = null;
                UserLogin tutor = null;

                verification = "VALID";

                notification.Payment_Gross = notification.Mc_Gross;
                if (notification.Subcr_Id != null)
                    notification.Txn_Id = notification.Subcr_Id;

                if (verification.Equals("INVALID"))
                    error += " INVALID";

                //if (await homeHelperFunctions.GetPayment(storageConfig, "PaymentsById", notification.Txn_Id) != null)
                //    error += " DUPLICATE";

                if (notification.Txn_Type == null || (!notification.Txn_Type.Equals("subscr_signup") && !notification.Txn_Type.Equals("web_accept")))
                    error += " INVALID_TXN_TYPE=" + notification.Txn_Type;

                if (notification.Item_Name.Equals("SUBSCRIPTION"))
                {
                    // THIS CODE IS CURRENTLY OUT OF USE, BUT MIGHT BE USED IN THE FUTURE!
                    // If you are debugging this code in the future, note that errors may occur when subscribed accounts no longer exist
                    subscription = true;

                    student = await _homeHelperFunctions.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, notification.Custom);
                    if (student == null)
                        error += " INVALID_STUDENT";
                }
                else
                {
                    string[] users = notification.Custom.Split('+');
                    if (users.Length < 2)
                        error += " NO_TUTOR";
                    else
                    {
                        student = await _homeHelperFunctions.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, users[0]);
                        if (student == null)
                            error += " INVALID_STUDENT";

                        tutor = await _homeHelperFunctions.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, users[1]);
                        if (tutor == null)
                            error += " INVALID_TUTOR";
                    }
                }

                if (notification.Payment_Status != null && !notification.Payment_Status.Equals("Completed"))
                    error += " INVALID_PAYMENT_STATUS=" + notification.Payment_Status;

                if (!notification.Receiver_Email.ToLower().Equals("streamworktutor@gmail.com"))
                    error += " WRONG_RECEIVER=" + notification.Receiver_Email;

                if (!notification.Residence_Country.Equals("US"))
                    error += " WRONG_RECEIVER_COUNTRY=" + notification.Residence_Country;

                if (notification.Test_Ipn)
                    error += " TEST";

                if (error.Equals(""))
                {
                    if (subscription)
                    {
                        student.Expiration = DateTime.UtcNow.AddMonths(1);

                        student.TrialAccepted = true;
                        await _homeHelperFunctions.UpdateUser(storageConfig, student);
                    }
                    else
                    {
                        tutor.Balance += notification.Payment_Gross * 0.9m;
                        await _homeHelperFunctions.UpdateUser(storageConfig, tutor);
                    }
                }
                else
                    await _emailHelperFunctions.SendEmailToAnyEmailAsync("streamworktutor@gmail.com", "thansenaz@icloud.com", null, "StreamWork IPN Error", error, null);

                await _homeHelperFunctions.SavePayment(storageConfig, new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = notification.Txn_Id ?? Guid.NewGuid().ToString(),
                    PayerEmail = notification.Payer_Email,
                    Student = student.Username,
                    Tutor = tutor?.Username,
                    PaymentType = notification.Item_Name,
                    Val = notification.Payment_Gross,
                    TimeSent = DateTime.UtcNow,
                    Verified = error.Equals(""),
                    Error = error
                });
            }
            catch (Exception e)
            {
                await _emailHelperFunctions.SendEmailToAnyEmailAsync("streamworktutor@gmail.com", "thansenaz@icloud.com", null, "StreamWork IPN Crash", "error: " + e.ToString(), null);
                await _homeHelperFunctions.SavePayment(storageConfig, new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = notification == null ? "NONE" : (notification.Txn_Id ?? "NONE"),
                    PayerEmail = notification == null ? "NONE" : (notification.Payer_Email ?? "NONE"),
                    Student = notification == null ? "NONE" : (notification.Custom ?? "NONE"),
                    Tutor = notification == null ? "NONE" : (notification.Custom ?? "NONE"),
                    PaymentType = notification == null ? "NONE" : (notification.Item_Name ?? "NONE"),
                    Val = notification == null ? 0 : notification.Payment_Gross,
                    TimeSent = DateTime.UtcNow,
                    Verified = false,
                    Error = e.ToString()
                });
            }

            return Ok();
        }
    }
}