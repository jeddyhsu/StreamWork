using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperClasses;

namespace StreamWork.Hubs
{
    public class ChatClient
    {
        private readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();

        public async Task<bool> SaveMessage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId, string userId, string name, string message, string profilePicture, DateTime dateTime)
        {
            try
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUserUsingId, userId);
                Chats chat = new Chats
                {
                    Id = Guid.NewGuid().ToString(),
                    ChatId = chatId,
                    UserId = userId,
                    Name = name,
                    Message = message,
                    ProfilePicture = profilePicture,
                    Date = dateTime
                };

                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", chat.Id } }, chat);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SaveMessage: " + e.Message);
                return false;
            }
        }
    }
}
