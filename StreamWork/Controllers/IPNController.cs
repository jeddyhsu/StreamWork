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
            }

            //Store the IPN received from PayPal
            Task.Run(() => LogRequest(ipnContext));

            //Fire and forget verification task
            VerifyTask();

            //Reply back a 200 code
            return Ok();
        }

        private void VerifyTask () {
            ProcessVerificationResponse();
        }


        private async Task LogRequest (IPNContext ipnContext) {
            // Persist the request values into a database or temporary data store

            await helperFunctions.LogIPNRequest(storageConfig, new IPNRequestBody {
                Id = Guid.NewGuid().ToString(),
                RequestBody = ipnContext.RequestBody
            });
        }

        private void ProcessVerificationResponse () { }
    }
}