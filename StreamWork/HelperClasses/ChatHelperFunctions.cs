using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;

namespace StreamWork.HelperClasses
{
    public class ChatHelperFunctions
    {
        HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();

        public async Task<List<Chats>> GetAllChatsWithChatId([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId)
        {
            var chats = await DataStore.GetListAsync<Chats>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetAllChatsFromSpecificId.ToString(), new List<string> { chatId });
            return chats;
        }

        public async Task DeleteAllChatsWithChatId([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId)
        {
            var chats = await DataStore.GetListAsync<Chats>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.DeleteAllChatsFromSpecificId.ToString(), new List<string> { chatId });
        }
    }
}
