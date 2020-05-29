using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.HelperClasses;
using StreamWork.ViewModels;

namespace StreamWork.Controllers
{
    public class ChatController : Controller
    {
        private readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        private readonly ChatHelperFunctions _chatHelperFunctions = new ChatHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> StreamWorkChat([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId)
        {
            bool isLoggedIn = true;
            if (HttpContext.User.Identity.Name == null)
                isLoggedIn = false;

            string chatColor = "";
            foreach(var claims in HttpContext.User.Claims)
            {
                if (claims.Type == ClaimTypes.UserData) chatColor = claims.Value;
            }

            ChatViewModel chatViewModel = new ChatViewModel
            {
                UserProfile = isLoggedIn == false ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name),
                ChatId = chatId,
                Chats = await _chatHelperFunctions.GetAllChatsWithChatId(storageConfig, chatId),
                ChatColor = chatColor,
                IsLoggedIn = isLoggedIn
            };

            return View(chatViewModel);
        }
    }
}
