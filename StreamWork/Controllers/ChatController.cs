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
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly ChatMethods _chatMethods = new ChatMethods();
        private readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();

        [HttpGet]
        public async Task<IActionResult> StreamWorkChat([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId, string chatInfo)
        {
            UserLogin userProfile = null;
            string[] info = null;

            if (chatInfo != null)
            {
                var decryptchatInfo = _encryptionMethods.DecryptString(chatInfo);
                info = decryptchatInfo.Split("|");
                userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, info[0]);
            }

            if(info != null)
                if (!(userProfile.Id == info[1] && userProfile.EmailAddress == info[2])) userProfile = null;

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
                Chats = await _chatMethods.GetAllChatsWithChatId(storageConfig, chatId),
                ChatColor = chatColor,
                IsLoggedIn = userProfile != null,
            };

            return View(chatViewModel);
        }
    }
}
