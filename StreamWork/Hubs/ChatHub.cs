using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StreamWork.Config;

namespace StreamWork.Hubs
{
    public class ChatHub : Hub
    {
        ChatClient _chatClient = new ChatClient();
        private IServiceProvider _sp;
        private static long questionCount = 1;

        public ChatHub(IServiceProvider sp)
        {
            _sp = sp; 
        }

        public Task JoinChatRoom(string chatId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessageToChatRoom(string chatId, string userId, string name, string message, string profilePicture)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", name.Replace('|',' '), message, profilePicture, questionCount);
            using (var scope = _sp.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<StorageConfig>>();
                await _chatClient.SaveMessage(dbContext, chatId, userId, name, message, profilePicture);
            }
        }
    }
}
