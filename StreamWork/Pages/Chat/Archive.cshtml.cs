using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Chat
{
    public class ArchiveChat : PageModel
    {
        private readonly StorageService storageService;
        private readonly CookieService cookieService;
        private readonly ChatService chatService;
        private readonly EncryptionService encryptionService;

        public string ChatId { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public DataModels.Profiles CurrentUserProfile { get; set; }
        public double Offset { get; set; }

        public ArchiveChat(StorageService storage, CookieService cookie, ChatService chat, EncryptionService encryption)
        {
            cookieService = cookie;
            chatService = chat;
            encryptionService = encryption;
            storageService = storage;
        }

        public async Task<IActionResult> OnGet(string streamId)
        {
            var stream = await storageService.Get<Video>(SQLQueries.GetArchivedStreamsWithId, streamId);
            //Chats = await chatService.GetAllChatsWithChatId(stream.Username, streamId);
            CurrentUserProfile = await cookieService.GetCurrentUser();

            Offset = CurrentUserProfile != null && CurrentUserProfile.TimeZone != null && CurrentUserProfile.TimeZone != "" ? MiscHelperMethods.GetOffsetBasedOfTimeZone(CurrentUserProfile.TimeZone) : -1.0;

            return Page();
        }
    }
}