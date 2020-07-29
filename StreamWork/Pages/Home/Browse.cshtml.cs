using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class BrowseModel : PageModel
    {
        public UserLogin CurrentUserProfile { get; set; }

        private readonly CookieService cookieService;

        public BrowseModel(CookieService cookie)
        {
            cookieService = cookie;
        }

        public IActionResult OnGet()
        {
            //CurrentUserProfile 
            return Page();
        }
    }
}
