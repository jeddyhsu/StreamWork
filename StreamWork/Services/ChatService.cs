using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class ChatService : StorageService
    {
        public ChatService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }
        
        public async Task<List<Chat>> GetAllChatsWithChatId(string chatId, string archivedVideoId)
        {
            return await GetList<Chat>(SQLQueries.GetAllChatsWithIdAndVideoId, new string[]{chatId, archivedVideoId});
        }

        public async Task DeleteAllChatsWithChatId(string chatId)
        {
            await Run<Chat>(SQLQueries.DeleteAllChatsWithId, new string[] { chatId });
        }

        public string GetRandomChatColor()
        {
            var random = new Random();
            var list = new List<string> { "#D9534F", "#F0AD4E", "#56C0E0", "#5CB85C", "#1C7CD5", "#8B4FD9" };
            int index = random.Next(list.Count);
            return list[index];
        }
    }
}
