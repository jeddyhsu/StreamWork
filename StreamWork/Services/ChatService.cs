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
            //return await GetList<Chat>(SQLQueries.GetAllChatsWithIdAndVideoId, new string[]{chatId, archivedVideoId});
            return null;
        }

        public async Task DeleteAllChatsWithChatId(string chatId)
        {
            //await Run<Chat>(SQLQueries.DeleteAllChatsWithId, new string[] { chatId });
        }

        public async Task DeleteAllChatsWithArchiveVideoId(string archiveVideoId)
        {
            //await Run<Chat>(SQLQueries.DeleteAllChatsWithArchivedVideoId, new string[] { archiveVideoId });
        }
    }
}
