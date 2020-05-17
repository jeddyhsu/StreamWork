﻿using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StreamWork.Config;

namespace StreamWork.Hubs
{
    public class ChatHub : Hub
    {
        private IServiceProvider _sp;
        private static Hashtable idTable = new Hashtable(); //associate tutorId with clientId
        private static long questionCount = 1;

        public ChatHub(IServiceProvider sp)
        {
            _sp = sp; 
        }

        public Task JoinChatRoom(string chatId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public Task SendMessageToChatRoom(string chatId, string user, string message, string questionType)
        {
            return Clients.Group(chatId).SendAsync("ReceiveMessage", user, message, questionType, questionCount);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message); //send message to all clients that have connected to the server
        }

        public async Task SendMessageToTutor(string studentName, string tutorId, string message, string questionType) //send message to specific tutorId
        {
            using (var scope = _sp.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot <StorageConfig>> ();
            }

            string tutorConnectionId = (string)idTable[tutorId];
            await Clients.Client(tutorConnectionId).SendAsync("ReceiveMessage", studentName, message, questionCount, questionType);
            questionCount++;

        }
    }
}
