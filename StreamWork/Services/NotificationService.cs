using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class NotificationService : StorageService
    {
        public NotificationService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public async Task<bool> SaveNotification(NotificationType notificationType, string senderUsername, string receiverUsername, string notficationInfo, string objectId)
        {
            var sender = await Get<UserLogin>(SQLQueries.GetUserWithUsername, senderUsername);
            var receiver = await Get<UserLogin>(SQLQueries.GetUserWithUsername, receiverUsername);

            try
            {
                Notification notification = new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderName = sender.Name,
                    SenderUsername = sender.Username,
                    SenderProfilePicture = sender.ProfilePicture,
                    ReceiverName = receiver.Name,
                    ReceiverUsername = receiver.Username,
                    Message = CreateNotification(notificationType),
                    Seen = "false",
                    Date = DateTime.UtcNow,
                    Type = notificationType.ToString(),
                    NotificationInfo = notficationInfo,
                    ObjectId = objectId,
                    ProfileColor = sender.ProfileColor
                };

                await Save(notification.Id, notification);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in NotificationService: CreateNotification" + e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteNotificationWhenObjectIsDeleted(string objectId)
        {
            return await Run<Notification>(SQLQueries.DeleteNotificationWithObjectId, objectId);
        }

        public async Task<bool> DeleteNotification(string id)
        {
            return await Run<Notification>(SQLQueries.DeleteNotificationWithId, id);
        }

        public async Task<List<Notification>> GetNotifications(string username)
        {
            return await GetList<Notification>(SQLQueries.GetNotificationsWithReceiver,username);
        }

        public async Task<bool> UpdateNotificationsToSeen(string username)
        {
            return await Run<Notification>(SQLQueries.UpdateNotificationToSeen, username);
        }

        public async Task<bool> AreThereUnseenNotifications(string username)
        {
            var unseenNotifications = await GetList<Notification>(SQLQueries.GetUnseenNotifications, username);

            if (unseenNotifications.Count > 0) return true;
            return false;
        }

        private string CreateNotification(NotificationType notificationType)
        {
            string message;

            if (notificationType == NotificationType.Comment)
            {
                message = " commented on your stream ";
            }
            else if(notificationType == NotificationType.Reply)
            {
                message = " replied to your comment ";
            }
            else
            {
                message = " started following you";
            }

            return message;
        }
    }
}
