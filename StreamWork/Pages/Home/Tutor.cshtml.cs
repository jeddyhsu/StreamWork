using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class TutorModal : PageModel
    {
        private readonly CookieService session;
        private readonly StorageService storage;

        public List<UserLogin> StreamTutors { get; set; }

        public TutorModal(CookieService session, StorageService storage)
        {
            this.session = session;
            this.storage = storage;
        }

        public async Task OnGet()
        {
            StreamTutors = await storage.GetList<UserLogin>(SQLQueries.GetAllApprovedTutors);
        }
    }
}
