using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class LoginModel : PageModel
    {
        private readonly SessionService session;
        private readonly StorageService storage;
        private readonly EncryptionService encryption;

        public LoginModel(SessionService session, StorageService storage, EncryptionService encryption)
        {
            this.session = session;
            this.storage = storage;
            this.encryption = encryption;
        }

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }
    }
}
