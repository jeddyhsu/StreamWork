﻿using System.Threading.Tasks;
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
            if(HttpContext.User.Identity.Name == null)
                return Redirect(_homeHelperFunctions._host + "/Home/Login?dest=-Chat-StreamWorkChat?chatId=" + chatId);

            ChatViewModel chatViewModel = new ChatViewModel
            {
                UserProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, HttpContext.User.Identity.Name),
                ChatId = chatId,
                Chats = await _chatHelperFunctions.GetAllChatsWithChatId(storageConfig, chatId)
            };

            return View(chatViewModel);
        }
    }
}
