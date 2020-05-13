using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StreamWork.Hubs
{
    public class ChatHub : Hub
    {
        private static Hashtable idTable = new Hashtable();

        public void RegisterConnection(string userId)
        {
            if (!idTable.Contains(userId)) idTable[userId] = Context.ConnectionId;
            else idTable.Add(userId, Context.ConnectionId);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToTutor(string studentName, string tutorId, string message)
        {
            string tutorConnectionId = (string)idTable[tutorId];
            await Clients.User(tutorConnectionId).SendAsync("ReceiveMessage", studentName, message);
        }
    }
}
