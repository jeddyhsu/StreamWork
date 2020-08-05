using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Chat
{
    public class LiveChat : PageModel
    {
        private readonly CookieService cookieService;
        private readonly ChatService chatService;

        public DataModels.Profile CurrentUserProfile { get; set; }
        public string ChatId { get; set; }
        public string ChatInfo { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public string ChatColor { get; set; }
        public bool IsLoggedIn { get; set; }

        public LiveChat(CookieService cookie, ChatService chat)
        {
            cookieService = cookie;
            chatService = chat;
        }

        public async Task<IActionResult> OnGet(string chatId, string chatInfo)
        {
            CurrentUserProfile = await cookieService.GetCurrentUser();
            ChatId = chatId;
            ChatInfo = chatInfo;
            Chats = await chatService.GetAllChatsWithChatId(ChatId);
            ChatColor = chatService.GetRandomChatColor();

            IsLoggedIn = CurrentUserProfile == null ? false : true;

            return Page();
        }
    }
}
