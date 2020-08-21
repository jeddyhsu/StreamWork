using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Hubs
{
    public class ChatHub : Hub
    {
        private StorageService storageService;
        private readonly IServiceProvider _sp;
        private static long _questionCount = 1;

        public ChatHub(IServiceProvider sp)
        {
            _sp = sp;
            using var scope = _sp.CreateScope();
            storageService = scope.ServiceProvider.GetRequiredService<StorageService>();
        }

        public Task JoinChatRoom(string chatId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessageToChatRoom(string chatId, string userName, string name, string message, string profilePicture, string chatColor, DateTime date, int offset, string archivedVideoId)
        {
            archivedVideoId = (await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, userName)).ArchivedVideoId;
            message = URLIFY(message);
            string chat = Serialize(chatId, userName, name, message, profilePicture, date, offset, chatColor, archivedVideoId);
            await Clients.Group(chatId).SendAsync("ReceiveMessage", chat);
            _questionCount++;
            await SaveMessage(chatId, userName, name, message, profilePicture, date, offset, chatColor, archivedVideoId);
        }

        private async Task<bool> SaveMessage(string chatId, string userName, string name, string message, string profilePicture, DateTime dateTime, int offset, string chatColor, string archivedVideoId)
        {
            try
            {
                if(archivedVideoId != null)
                {
                    var userProfile = await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, userName);
                    Chat chat = new Chat
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
                        ArchivedVideoId = archivedVideoId,
                    };

                    await storageService.Save(chat.Id, chat);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in SaveMessage: " + e.Message);
                return false;
            }
        }


        private string URLIFY(string message)
        {
            string pattern = "(https?://([^ ]+))";
            string replacement = "<a target=\"_blank\" href=\"$1\">$2</a>";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(message, replacement);

            return result;
        }

        private string Serialize(string chatId, string userName, string name, string message, string profilePicture, DateTime dateTime, int offset, string chatColor, string archivedVideoId)
        {
            Chat chat = new Chat
            {
                ChatId = chatId,
                Username = userName,
                Name = name,
                Message = message,
                ProfilePicture = profilePicture,
                Date = dateTime,
                ChatColor = chatColor,
                TimeOffset = offset,
                ArchivedVideoId = archivedVideoId
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(chat);
        }
    }
}
