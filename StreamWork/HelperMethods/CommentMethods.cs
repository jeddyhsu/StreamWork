using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;

namespace StreamWork.HelperMethods
{
    public class CommentMethods
    {
        readonly HomeMethods _homeMethods = new HomeMethods();

        public async Task SaveComment([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string senderUsername, string receiverUsername, string message)
        {
            var sender = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, senderUsername);
            var receiver = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, receiverUsername);

            Comment comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                SenderName = sender.Name,
                SenderUsername = sender.Username,
                SenderProfilePicture = sender.ProfilePicture,
                ReceiverName = receiver.Name,
                ReceiverUsername = receiver.Username,
                Message = message,
                Date = DateTime.UtcNow
            };

            await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", comment.Id } }, comment);
        }

        public async Task<List<Comment>> GetComments([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username)
        {
            return await DataStore.GetListAsync<Comment>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetCommentsWithReceiverUsername.ToString(), new List<string> { username });
        }
    }
}
