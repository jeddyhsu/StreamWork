using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Chat
{
    public class ArchiveChat : PageModel
    {
        private readonly CookieService cookieService;
        private readonly ChatService chatService;
        private readonly EncryptionService encryptionService;

        public string ChatId { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public Profile UserProfile { get; set; }

        public ArchiveChat(CookieService cookie, ChatService chat, EncryptionService encryption)
        {
            cookieService = cookie;
            chatService = chat;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string chatId, string chatInfo)
        {
            ChatId = chatId;
            Chats = await chatService.GetAllChatsWithChatId(chatId, encryptionService.DecryptString(chatInfo));
            UserProfile = await cookieService.GetCurrentUser();

            return Page();
        }
    }
}