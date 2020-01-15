using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperClasses;
using StreamWork.Models;

namespace StreamWork.Controllers {
    public class WebhooksController : Controller {
        private IOptionsSnapshot<StorageConfig> storageConfig;
        private readonly HomeHelperFunctions homehelperFunctions = new HomeHelperFunctions();

        // PayPal notification webhook
        [HttpPost]
        public IActionResult WebhookCD0B367C070C81B0 ([FromBody] PayPalNotification notification, [FromServices] IOptionsSnapshot<StorageConfig> _storageConfig) { // Keep the name of this method secret
            storageConfig = _storageConfig;
            return VerifyPayPalNotification(notification).Result;
        }

        private async Task<IActionResult> VerifyPayPalNotification (PayPalNotification notification) {
            string error = "";

            // Do verification here

            await ProcessPayPalNotification(notification, error);

            return Ok();
        }

        private async Task<IActionResult> ProcessPayPalNotification (PayPalNotification notification, string error) {
            //await homehelperFunctions.SendEmailToAnyEmailAsync("streamworktutor@gmail.com", "thansenaz@icloud.com", "StreamWork Notification 5", "Resource: " + notification.Resource, new List<System.Net.Mail.Attachment>());
            try {
                if (await homehelperFunctions.GetPayment(storageConfig, "PaymentsById", notification.Id) != null) {
                    error = "DUPLICATE " + error;
                }

                await homehelperFunctions.SavePayment(storageConfig, new Payment {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = notification.Id,
                    PayerEmail = "test@test.test",
                    Student = "test",
                    Tutor = "test",
                    PaymentType = "test",
                    Val = 0,
                    TimeSent = DateTime.UtcNow,
                    Verified = "test",
                    Error = error
                });
            }
            catch (Exception e) {
                await homehelperFunctions.SavePayment(storageConfig, new Payment {
                    Id = Guid.NewGuid().ToString(),
                    TransactionId = "test",
                    PayerEmail = "test@test.test",
                    Student = "test",
                    Tutor = "test",
                    PaymentType = "test",
                    Val = 0,
                    TimeSent = DateTime.UtcNow,
                    Verified = "test",
                    Error = e.ToString()
                });
            }

            return Ok();
        }
    }
}