using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class CommentService : StorageService
    {
        public CommentService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public async Task<List<string>> SaveComment(string senderUsername, string receiverUsername, string message, string parentId, string streamId)
        {
            try
            {
                var sender = await Get<Profile>(SQLQueries.GetUserWithUsername, senderUsername);
                var receiver = await Get<Profile>(SQLQueries.GetUserWithUsername, receiverUsername);

                Comment comment = new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderName = sender.Name,
                    SenderUsername = sender.Username,
                    SenderProfilePicture = sender.ProfilePicture,
                    ReceiverName = receiver.Name,
                    ReceiverUsername = receiver.Username,
                    Message = MiscHelperMethods.URLIFY(message),
                    ParentId = parentId,
                    Date = DateTime.UtcNow,
                    StreamId = streamId,
                    ProfileColor = sender.ProfileColor,
                };

                await Save(comment.Id, comment);
                return new List<string> { sender.ProfilePicture, sender.Name.Replace('|', ' '), comment.Message, comment.Id, comment.ReceiverName.Replace('|', ' '), comment.ProfileColor };
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in SaveComment " + e.Message);
                return null;
            }
        }

        public async Task<List<string>> EditComment(string message, string commentId)
        {
            try
            {
                var comment = await Get<Comment>(SQLQueries.GetCommentWithId, new string[] { commentId });
                comment.Message = MiscHelperMethods.URLIFY(message);
                comment.Edited = "true";
                await Save<Comment>(comment.Id, comment);
                return new List<string> {comment.Message, comment.Id};
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in EditComment " + e.Message);
                return null;
            }
        }

        public async Task<bool> DeleteComment(string commentId)
        {
            try
            {
                var comment = await Get<Comment>(SQLQueries.GetCommentWithId, new string[] { commentId });
                return await Run<Comment>(SQLQueries.DeleteComment, new string[] { commentId });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in RemoveComment " + e.Message);
                return false;
            }
        }

        public async Task<List<Comment>> GetAllComments(string streamId)
        {
            var comments = await GetList<Comment>(SQLQueries.GetCommentsWithStreamId, new string[] { streamId });
            var dictonary = comments.ToDictionary(v => v.Id, v => v.Replies);
            var replies = await GetList<Comment>(SQLQueries.GetRepliesWithStreamId, new string[] { streamId });

            foreach (var reply in replies)
            {
                dictonary[reply.ParentId].Add(reply);
            }

            return comments;
        }
    }
}
