using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperClasses;
using StreamWork.Hubs;
using StreamWork.ViewModels;

namespace StreamWork.Controllers
{
    public class ChatController : Controller
    {
        private readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        private IHubContext<ChatHub> _hubContext;

        [HttpGet]
        public async Task<IActionResult> TutorQuestionChat([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            if(HttpContext.User.Identity.Name == null)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Chat-TutorQuestionChat");   

            ChatViewModel chatViewModel = new ChatViewModel
            {
                TutorChannel = await _homeHelperFunctions.GetUserChannel(storageConfig, QueryHeaders.CurrentUserChannel, HttpContext.User.Identity.Name)
            };
            return View(chatViewModel);
        }
    }
}
