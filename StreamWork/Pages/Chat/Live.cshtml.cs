using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Chat
{
    public class Live : PageModel
    {
        private readonly StorageService storageService;
        private readonly CookieService cookieService;
        private readonly ChatService chatService;
        private readonly EncryptionService encryptionService;

        public Profile CurrentUserProfile { get; set; }
        public string ChatId { get; set; }
        public string ChatInfo { get; set; }
        public string ChatInfoEncrypted { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public bool IsLoggedIn { get; set; }

        public Live(StorageService storage, CookieService cookie, ChatService chat, EncryptionService encryption)
        {
            storageService = storage;
            cookieService = cookie;
            chatService = chat;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string chatId)
        {
            var channel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, chatId);

            CurrentUserProfile = await cookieService.GetCurrentUser();
            ChatId = chatId;
            ChatInfo = channel.ArchivedVideoId ?? "DEFAULT";
            Chats = await chatService.GetAllChatsWithChatId(ChatId, ChatInfo);
            IsLoggedIn = CurrentUserProfile == null ? false : true;

            return Page();
        }
    }
}
