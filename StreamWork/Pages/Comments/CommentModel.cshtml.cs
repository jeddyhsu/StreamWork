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

        public async Task<IActionResult> OnPostSaveComment(string senderUsername, string receiverUsername, string message, string parentId, string streamId)
        {
            var savedInfo = await commentService.SaveComment(senderUsername, receiverUsername, message, parentId, streamId);
            var savedNotification = await  notificationService.SaveNotification(NotificationType.Comment, senderUsername, receiverUsername, (await storageService.Get<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithStreamId, streamId )).StreamTitle + "|" + streamId + "|" + savedInfo[2], savedInfo[3]);
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
            if (savedInfo) return new JsonResult(new { Message = JsonResponse.Success.ToString()});

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
