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
        readonly ChatClient _chatClient = new ChatClient();
        private readonly IServiceProvider _sp;
        private static long _questionCount = 1;

        public ChatHub(IServiceProvider sp)
        {
            _sp = sp; 
        }

        public Task JoinChatRoom(string chatId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessageToChatRoom(string chatId, string userName, string name, string message, string profilePicture, string chatColor, DateTime date, int offset)
        {
            message = _chatClient.URLIFY(message);
            await Clients.Group(chatId).SendAsync("ReceiveMessage", name.Replace('|',' '), message, profilePicture, _questionCount, userName, chatColor);
            using (var scope = _sp.CreateScope())
            {
                _questionCount++;
                var dbContext = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<StorageConfig>>();
                await _chatClient.SaveMessage(dbContext, chatId, userName, name, message, profilePicture, date, offset, chatColor);
            }
        }
    }
}
