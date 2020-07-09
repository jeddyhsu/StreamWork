using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Comment
{
    public class CommentModel : PageModel
    {
        private readonly CommentService commentService;

        public CommentModel(CommentService comment)
        {
            commentService = comment;
        }

        public async Task<IActionResult> OnPostSaveComment(string senderUsername, string receiverUsername, string message, string parentId, string streamId)
        {
            var savedInfo = await commentService.SaveComment(senderUsername, receiverUsername, message, parentId, streamId);
            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo});

            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
