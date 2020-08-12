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
        private readonly EncryptionService encryptionService;

        public Profile CurrentUserProfile { get; set; }
        public string ChatId { get; set; }
        public string ChatInfo { get; set; }
        public string ChatInfoEncrypted { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public string ChatColor { get; set; }
        public bool IsLoggedIn { get; set; }

        public LiveChat(CookieService cookie, ChatService chat, EncryptionService encryption)
        {
            cookieService = cookie;
            chatService = chat;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string chatId, string chatInfo)
        {
            CurrentUserProfile = await cookieService.GetCurrentUser();
            ChatId = chatId;
            ChatInfo = encryptionService.DecryptString(chatInfo);
            ChatInfoEncrypted = chatInfo;
            Chats = await chatService.GetAllChatsWithChatId(ChatId, ChatInfo);
            ChatColor = chatService.GetRandomChatColor();

            IsLoggedIn = CurrentUserProfile == null ? false : true;

            return Page();
        }
    }
}
