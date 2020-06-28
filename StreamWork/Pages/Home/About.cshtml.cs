using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class AboutModel : PageModel
    {
        private readonly SessionService session;

        public UserLogin GenericUserProfile { get; set; }

        public AboutModel(SessionService session)
        {
            this.session = session;
        }

        public async void OnGet()
        {
            GenericUserProfile = await session.GetCurrentUser();
        }
    }
}
