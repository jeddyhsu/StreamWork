using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperClasses;
using StreamWork.ViewModels;

namespace StreamWork.Controllers
{
    public class ChatController : Controller
    {
        private readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        private readonly ChatHelperFunctions _chatHelperFunctions = new ChatHelperFunctions();

        [HttpGet]
        public async Task<IActionResult> StreamWorkChat([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId, string chatInfo)
        {
            var decryptchatInfo = _homeHelperFunctions.DecryptString(chatInfo);
            UserLogin userProfile = null;
            if(chatInfo != null) userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, decryptchatInfo);

            string chatColor = "";
            foreach(var claims in HttpContext.User.Claims)
            {
                if (claims.Type == ClaimTypes.UserData) chatColor = claims.Value;
            }

            ChatViewModel chatViewModel = new ChatViewModel
            {
                UserProfile = userProfile,
                ChatId = chatId,
                ChatInfo = chatInfo ?? null,
                Chats = await _chatHelperFunctions.GetAllChatsWithChatId(storageConfig, chatId),
                ChatColor = chatColor,
                IsLoggedIn = userProfile != null,
            };

            return View(chatViewModel);
        }
    }
}
