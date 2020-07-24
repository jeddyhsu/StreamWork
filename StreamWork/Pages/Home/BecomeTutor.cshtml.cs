using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class BecomeTutorModel : PageModel
    {
        private readonly CookieService session;
        private readonly StorageService storage;

        public UserLogin GenericUserProfile { get; set; }
        public List<UserLogin> StreamTutors { get; set; }

        public BecomeTutorModel(CookieService session, StorageService storage)
        {
            this.session = session;
            this.storage = storage;
        }

        public async Task OnGet()
        {
            GenericUserProfile = await session.GetCurrentUser();
            StreamTutors = await storage.GetList<UserLogin>(SQLQueries.GetAllApprovedTutors);
        }
    }
}
