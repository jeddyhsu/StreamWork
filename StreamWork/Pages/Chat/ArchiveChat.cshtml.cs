﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.Services;

namespace StreamWork.Pages.Chat
{
    public class ArchiveChat : PageModel
    {
        private readonly CookieService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly EditService editService;
        private readonly ChatService chatService;

        public string ChatId { get; set; }
        public List<DataModels.Chat> Chats { get; set; }
        public DataModels.Profile UserProfile { get; set; }

        public ArchiveChat(StorageService storage, CookieService session, ProfileService profile, ScheduleService schedule, FollowService follow, EditService edit, ChatService chat)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            editService = edit;
            chatService = chat;
        }

        public async Task<IActionResult> OnGet(string chatId)
        {
            ChatId = chatId;
            Chats = await chatService.GetAllChatsWithChatId(chatId);
            UserProfile = await sessionService.GetCurrentUser();

            return Page();
        }
    }
}