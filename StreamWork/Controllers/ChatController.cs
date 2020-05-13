using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.HelperClasses;
using StreamWork.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StreamWork.Controllers
{
    public class ChatController : Controller
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();

        public async Task<IActionResult> TutorQuestionChat([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            ChatViewModel chatViewModel = new ChatViewModel
            {
                TutorChannel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, HttpContext.User.Identity.Name)
            };
            return View(chatViewModel);
        }
    }
}
