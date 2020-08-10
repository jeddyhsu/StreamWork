using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StreamWork.Hubs
{
    public class PushNotificationHub : Hub
    {
        private readonly IServiceProvider sp;

        public PushNotificationHub(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }
    }
}
