using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class SignInModel : PageModel
    {
        private readonly EncryptionService encryptionService;
        private readonly CookieService cookieService;

        public SignInModel(EncryptionService encryption, CookieService cookie)
        {
            encryptionService = encryption;
            cookieService = cookie;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostSignIn()
        {
            var username = Request.Form["Username"];
            var password = Request.Form["Password"];

            var userProfile = await cookieService.ValidateUser(username);
            if(userProfile != null)
            {
                var signInProfile = await cookieService.SignIn(username, encryptionService.DecryptPassword(userProfile.Password, password));
                if (signInProfile != null)
                {
                    if(signInProfile.ProfileType == "tutor")
                        return new JsonResult(new { Message = JsonResponse.Tutor.ToString() });
                    else
                        return new JsonResult(new { Message = JsonResponse.Student.ToString() });
                }
                else
                {
                    return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
                }
            }

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSignOut()
        {
            await cookieService.SignOut();
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
