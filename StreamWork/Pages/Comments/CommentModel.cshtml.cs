using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Comments
{
    public class CommentModel : PageModel
    {
        private readonly StorageService storageService;
        private readonly CommentService commentService;
        private readonly NotificationService notificationService;
        
        public CommentModel(StorageService storage, CommentService comment, NotificationService notification)
        {
            storageService = storage;
            commentService = comment;
            notificationService = notification;
        }

        public async Task<IActionResult> OnPostSaveComment(string senderUsername, string receiverUsername, string message, string parentId, string masterParent, string streamId)
        {
            if (parentId != null)
            {
                var parentComment = await storageService.Get<Comment>(SQLQueries.GetCommentWithId, parentId);
                receiverUsername = (await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, parentComment.SenderUsername)).Username; //this could be a reply to a reply so receiver could change
            }

            var savedInfo = await commentService.SaveComment(senderUsername, receiverUsername, message, masterParent == "undefined" ? parentId: masterParent, streamId);
            var savedNotification = await  notificationService.SaveNotification(parentId  == null ? NotificationType.Comment : NotificationType.Reply, senderUsername, receiverUsername, parentId == null ? (await storageService.Get<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithStreamId, streamId )).StreamTitle + "|" + streamId + "|" + savedInfo[2] : (await storageService.Get<Comment>(SQLQueries.GetCommentWithId, parentId)).Message +  "|" + streamId + "|" + savedInfo[2], savedInfo[3]); //savedInfo[2] == comment message
            if (savedInfo != null && savedNotification) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo});

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostEditComment(string message, string commentId)
        {
            var savedInfo = await commentService.EditComment(message, commentId);
            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo });

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteComment(string commentId)
        {
            var savedInfo = await commentService.DeleteComment(commentId);
            var deletedNotification = await notificationService.DeleteNotificationWhenObjectIsDeleted(commentId);

            if (savedInfo && deletedNotification) return new JsonResult(new { Message = JsonResponse.Success.ToString()});

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
