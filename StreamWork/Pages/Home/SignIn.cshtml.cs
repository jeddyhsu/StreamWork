using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class SignInModel : PageModel
    {
        private readonly StorageService storage;
        private readonly EncryptionService encryptionService;
        private readonly CookieService cookieService;

        public SignInModel(StorageService storage, EncryptionService encryption, CookieService cookie)
        {
            this.storage = storage;
            encryptionService = encryption;
            cookieService = cookie;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostSignIn(string route)
        {
            var username = Request.Form["Username"];
            var password = Request.Form["Password"];

            var userProfile = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, username);
            if (userProfile == null)
            {
                // User can also sign in with email address, in which case the username needs to be updated
                userProfile = await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, username);
                if (userProfile != null)
                {
                    username = userProfile.Username;
                }
            }

            if (userProfile != null)
            {
                var signInProfile = await cookieService.SignIn(username, encryptionService.DecryptPassword(userProfile.Password, password));
                if (signInProfile != null)
                {
                    if(route != "SW")
                    {
                        var decryptedRoute = encryptionService.DecryptString(route);
                        return new JsonResult(new { Message = "Route", Route = decryptedRoute });
                    }
                    else if(signInProfile.ProfileType == "tutor")
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
