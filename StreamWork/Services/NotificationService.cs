using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Hubs;

namespace StreamWork.Services
{
    public class NotificationService : StorageService
    {
        IHubContext<PushNotificationHub> hubContext;

        public NotificationService([FromServices] IOptionsSnapshot<StorageConfig> config, IHubContext<PushNotificationHub> hub) : base(config)
        {
            hubContext = hub;
        }

        public async Task<bool> SaveNotification(NotificationType notificationType, string senderUsername, string receiverUsername, string objectId = null)
        {
            var sender = await Get<Profile>(SQLQueries.GetUserWithUsername, senderUsername);
            var receiver = await Get<Profile>(SQLQueries.GetUserWithUsername, receiverUsername);

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
                    Seen = "false",
                    Date = DateTime.UtcNow,
                    Type = notificationType.ToString(),
                    ObjectId = objectId,
                    ProfileColor = sender.ProfileColor
                };

                await hubContext.Clients.User(notification.ReceiverUsername).SendAsync("ReceiveNotification", await CreateNotificationTemplate(notification, false), await CreateNotificationTemplate(notification, true), notification.Id);
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

        public async Task<List<string>> GetNotifications(string username)
        {
           var notifications = await GetList<Notification>(SQLQueries.GetNotificationsWithReceiver,username);
           List<string> notificationTemplates = new List<string>();

            foreach (var notification in notifications) {
                var notififcation = await CreateNotificationTemplate(notification, false);
                if (notififcation != null)
                    notificationTemplates.Add(notififcation);
            }
           
           return notificationTemplates;
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

        public async Task<string> CreateNotificationTemplate(Notification notification, bool isPush)
        {
            string reader = "";

            if (notification.Type == NotificationType.Follow.ToString())
            {
                string template;
                
                if (!isPush) template = "NotificationTemplates/FollowNotification.html";
                else template = "NotificationTemplates/PushNotifications/FollowPushNotification.html";

                using (StreamReader streamReader = new StreamReader(template))
                {
                    reader = streamReader.ReadToEnd();
                    reader = reader.Replace("{NotificationId}", notification.Id);
                    reader = reader.Replace("{SenderProfilePicture}", notification.SenderProfilePicture);
                    reader = reader.Replace("{ProfileColor}", notification.ProfileColor);
                    reader = reader.Replace("{SenderName}", notification.SenderName.Replace('|', ' '));
                    reader = reader.Replace("{NotificationMessage}", " started following you");
                    reader = reader.Replace("{NotificationDate}", notification.Date.ToLocalTime().ToShortTimeString());
                    reader = reader.Replace("{SenderUsername}", notification.SenderUsername);
                    reader = reader.Replace("{NotificationType}", notification.Type);
                }
            }
            else if (notification.Type == NotificationType.Comment.ToString())
            {
                string template;

                if (!isPush) template = "NotificationTemplates/CommentNotification.html";
                else template = "NotificationTemplates/PushNotifications/CommentPushNotification.html";

                var comment = await Get<Comment>(SQLQueries.GetCommentWithId, notification.ObjectId);
                if (comment == null) return null;
                var video = await Get<Video>(SQLQueries.GetArchivedStreamsWithId, comment.StreamId);

                using (StreamReader streamReader = new StreamReader(template))
                {
                    reader = streamReader.ReadToEnd();
                    reader = reader.Replace("{NotificationId}", notification.Id);
                    reader = reader.Replace("{SenderProfilePicture}", notification.SenderProfilePicture);
                    reader = reader.Replace("{ProfileColor}", notification.ProfileColor);
                    reader = reader.Replace("{SenderName}", notification.SenderName.Replace('|', ' '));
                    reader = reader.Replace("{NotificationMessage}", " commented on your stream (" + video.StreamTitle + ")");
                    reader = reader.Replace("{CommentMessage}", comment.Message);
                    reader = reader.Replace("{VideoId}", video.StreamID); //we need to use videos Id rather than streamId becasue we cahnge streaming later it will be hard to manage
                    reader = reader.Replace("{CommentId}", comment.Id);
                    reader = reader.Replace("{NotificationDate}", notification.Date.ToLocalTime().ToShortTimeString());
                    reader = reader.Replace("{SenderUsername}", notification.SenderUsername);
                    reader = reader.Replace("{NotificationType}", notification.Type);
                }
            }
            else if (notification.Type == NotificationType.Reply.ToString())
            {
                string template;

                if (!isPush) template = "NotificationTemplates/ReplyNotification.html";
                else template = "NotificationTemplates/PushNotifications/ReplyPushNotification.html";

                var comment = await Get<Comment>(SQLQueries.GetCommentWithId, notification.ObjectId);
                if (comment == null) return null;
                var parentComment = await Get<Comment>(SQLQueries.GetCommentWithId, comment.ParentId);
                var video = await Get<Video>(SQLQueries.GetArchivedStreamsWithId, comment.StreamId);
                
                using (StreamReader streamReader = new StreamReader(template))
                {
                    reader = streamReader.ReadToEnd();
                    reader = reader.Replace("{NotificationId}", notification.Id);
                    reader = reader.Replace("{SenderProfilePicture}", notification.SenderProfilePicture);
                    reader = reader.Replace("{ProfileColor}", notification.ProfileColor);
                    reader = reader.Replace("{SenderName}", notification.SenderName.Replace('|', ' '));
                    reader = reader.Replace("{NotificationMessage}", " replied to your comment");
                    reader = reader.Replace("{CommentReply}", comment.Message);
                    reader = reader.Replace("{CommentMessage}", parentComment.Message);
                    reader = reader.Replace("{VideoId}", video.StreamID); //we need to use videos Id rather than streamId becasue we cahnge streaming later it will be hard to manage
                    reader = reader.Replace("{CommentId}", comment.Id);
                    reader = reader.Replace("{NotificationDate}", notification.Date.ToLocalTime().ToShortTimeString());
                    reader = reader.Replace("{SenderUsername}", notification.SenderUsername);
                    reader = reader.Replace("{NotificationType}", notification.Type);
                }
            }

            return reader;
        }
    }
}
