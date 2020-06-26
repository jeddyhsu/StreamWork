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
        
        public async Task<List<Chats>> GetAllChatsWithChatId([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId)
        {
            return await GetChats(SQLQueries.GetAllChatsWithId, new string[]{chatId});
        }

        public async Task DeleteAllChatsWithChatId([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId)
        {
            await Run<Chats>(SQLQueries.DeleteAllChatsWithId, new string[] { chatId });
        }
    }
}
