using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class AboutModel : PageModel
    {
        public AboutModel()
        {
           
        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
