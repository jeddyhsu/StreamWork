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

        public async Task<IActionResult> OnPostSaveComment(string senderUsername, string receiverUsername, string message, string parentId, string masterParent, string streamId) //streamId is videos actaul Id
        {
            if (parentId != null)
            {
                var parentComment = await storageService.Get<Comment>(SQLQueries.GetCommentWithId, parentId);
                receiverUsername = (await storageService.Get<DataModels.Profiles>(SQLQueries.GetUserWithUsername, parentComment.SenderUsername)).Username; //this could be a reply to a reply so receiver could change
            }

            var savedComment = await commentService.SaveComment(senderUsername, receiverUsername, message, masterParent == "undefined" ? parentId: masterParent, streamId);
            if(savedComment != null)
            {
                await notificationService.SaveNotification(parentId == null ? NotificationType.Comment : NotificationType.Reply,
                                                                                   senderUsername,
                                                                                   receiverUsername,
                                                                                   savedComment.Id);
                return new JsonResult(new { Message = JsonResponse.Success.ToString(), Comment = savedComment });
            }
                
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostEditComment(string message, string commentId)
        {
            var editedComment = await commentService.EditComment(message, commentId);
            if (editedComment != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Comment = editedComment });

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
