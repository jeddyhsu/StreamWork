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
        private readonly StorageService storageService;
        private readonly EncryptionService encryptionService;
        private readonly CookieService cookieService;

        public SignInModel(StorageService storage, EncryptionService encryption, CookieService cookie)
        {
            this.storageService = storage;
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
            var time = Request.Form["Time"]; //gets time offset

            var userProfile = await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, username);
            if (userProfile == null)
            {
                // User can also sign in with email address, in which case the username needs to be updated
                userProfile = await storageService.Get<Profile>(SQLQueries.GetUserWithEmailAddress, username);
                if (userProfile != null)
                {
                    username = userProfile.Username;
                }
            }

            if (userProfile != null)
            {
                if (time != "")
                {
                    userProfile.TimeZone = MiscHelperMethods.GetTimeZoneBasedOfOffset(time);
                    await storageService.Save(userProfile.Id, userProfile);
                }

                var signInProfile = await cookieService.SignIn(username, encryptionService.DecryptPassword(userProfile.Password, password));
                if (signInProfile != null)
                    return cookieService.Route(signInProfile, route, encryptionService);
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
