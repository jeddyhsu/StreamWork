using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Hubs
{
    public class ChatClient
    {
        private readonly HomeMethods _homeHelperFunctions = new HomeMethods();

        public async Task<bool> SaveMessage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string chatId, string userName, string name, string message, string profilePicture, DateTime dateTime, int offset, string chatColor)
        {
            try
            {
                var userProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, userName);
                Chats chat = new Chats
                {
                    Id = Guid.NewGuid().ToString(),
                    ChatId = chatId,
                    Username = userProfile.Username,
                    Name = name,
                    Message = message,
                    ProfilePicture = profilePicture,
                    Date = dateTime,
                    ChatColor = chatColor,
                    TimeOffset = offset,
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


        public string URLIFY(string message)
        {
            string pattern = "(https?://([^ ]+))";
            string replacement = "<a target=\"_blank\" href=\"$1\">$2</a>";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(message, replacement);

            return result;
        }
    }
}
