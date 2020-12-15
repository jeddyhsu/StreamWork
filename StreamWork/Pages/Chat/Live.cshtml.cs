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

        public DataModels.Profiles CurrentUserProfile { get; set; }
        public string ChatId { get; set; }
        public string ChatInfo { get; set; }
        public string ChatInfoEncrypted { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public bool IsLoggedIn { get; set; }
        public double Offset { get; set; }

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
            IsLoggedIn = CurrentUserProfile != null;

            Offset = CurrentUserProfile != null && CurrentUserProfile.TimeZone != null && CurrentUserProfile.TimeZone != "" ? MiscHelperMethods.GetOffsetBasedOfTimeZone(CurrentUserProfile.TimeZone) : -1.0;

            return Page();
        }
    }
}
