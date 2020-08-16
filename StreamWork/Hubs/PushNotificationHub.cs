using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StreamWork.DataModels;

namespace StreamWork.Hubs
{
    public class PushNotificationHub : Hub
    {
        private readonly IServiceProvider sp;

        public PushNotificationHub(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public Task SendPrivateMessage(string user, Notification notification)
        {
            return Clients.User(user).SendAsync("ReceiveNotification", notification);
        }
    }
}
