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
                var sender = await Get<UserLogin>(SQLQueries.GetUserWithUsername, senderUsername);
                var receiver = await Get<UserLogin>(SQLQueries.GetUserWithUsername, receiverUsername);

                Comment comment = new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderName = sender.Name,
                    SenderUsername = sender.Username,
                    SenderProfilePicture = sender.ProfilePicture,
                    ReceiverName = receiver.Name,
                    ReceiverUsername = receiver.Username,
                    Message = message,
                    ParentId = parentId,
                    Date = DateTime.UtcNow,
                    StreamId = streamId
                };

                await Save<Comment>(comment.Id, comment);
                return new List<string> { sender.ProfilePicture, sender.Name.Replace('|', ' '), message, comment.Id};
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in SaveComment " + e.Message);
                return null;
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
