using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class PayPalTestModel : PageModel
    {
        private readonly PayPalService payPal;

        public PayPalTestModel (PayPalService payPal)
        {
            this.payPal = payPal;
        }

        public async void OnGet()
        {
            await payPal.ProcessNewWebhooks();
        }
    }
}
