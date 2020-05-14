using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StreamWork.Hubs
{
    public class ChatHub : Hub
    {
        private static Hashtable idTable = new Hashtable(); //associate tutorId with clientId
        private static long questionCount = 1;

        public void RegisterConnection(string userId)
        {
            if (idTable.Contains(userId)) idTable[userId] = Context.ConnectionId; //register connection
            else idTable.Add(userId, Context.ConnectionId);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message); //send message to all clients that have connected to the server
        }

        public async Task SendMessageToTutor(string studentName, string tutorId, string message, string questionType) //send message to specific tutorId
        {
            string tutorConnectionId = (string)idTable[tutorId];
            await Clients.Client(tutorConnectionId).SendAsync("ReceiveMessage", studentName, message, questionCount, questionType);
            questionCount++;
        }
    }
}
