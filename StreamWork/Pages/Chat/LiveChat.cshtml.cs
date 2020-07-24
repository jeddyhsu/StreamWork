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
        private readonly CookieService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly EditService editService;
        private readonly ChatService chatService;

        public UserLogin UserProfile { get; set; }
        public string ChatId { get; set; }
        public string ChatInfo { get; set; }
        public List<Chats> Chats { get; set; }
        public string ChatColor { get; set; }
        public bool IsLoggedIn { get; set; }

        public LiveChat(StorageService storage, CookieService session, ProfileService profile, ScheduleService schedule, FollowService follow, EditService edit, ChatService chat)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            editService = edit;
            chatService = chat;
        }

        public async Task<IActionResult> OnGet(string chatId, string chatInfo)
        {
            UserProfile = await sessionService.GetCurrentUser();
            ChatId = chatId;
            ChatInfo = chatInfo;
            Chats = await chatService.GetAllChatsWithChatId(ChatId);
            ChatColor = chatService.GetRandomChatColor();
            IsLoggedIn = true;

            return Page();
        }
    }
}
