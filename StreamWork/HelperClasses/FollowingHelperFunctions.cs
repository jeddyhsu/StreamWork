using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;

namespace StreamWork.HelperClasses
{
    public class FollowingHelperFunctions //All helper functions that have to with students following tutors
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();

        public async Task<bool> AddFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            var followerProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUserUsingId, followerId);
            var followeeProfile = await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUserUsingId, followeeId);

            if (followerProfile != null && followeeProfile != null)
            {
                Follow followRequest = new Follow
                {
                    Id = Guid.NewGuid().ToString(),
                    FollowerId = followerProfile.Id,
                    FollowerUsername = followerProfile.Username,
                    FollowerEmail = followerProfile.EmailAddress,
                    FolloweeId = followeeProfile.Id,
                    FolloweeUsername = followeeProfile.Username,
                    FolloweeEmail = followeeProfile.EmailAddress,
                };

                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", followRequest.Id } }, followRequest);

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if(await DataStore.DeleteDataAsync<Follow>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.RemoveFollower.ToString(), new List<string> { followerId , followeeId })) return true;
            }

            return false;
        }

        public async Task<List<UserLogin>> GetAllFollowees([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId)
        {
            var listOfFollowees = await DataStore.GetListAsync<Follow>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetAllFollowersForSpecificId.ToString(), new List<string> { followerId });
            if(listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.GetFollowedLogins, _homeHelperFunctions.FormatQueryString(idList));
            }

            return null;
        }

        public async Task<List<UserLogin>> GetAllNonFollowees([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId) //all users that arent follwing the followee
        {
            var listOfFollowees = await DataStore.GetListAsync<Follow>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetAllFollowersForSpecificId.ToString(), new List<string> { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.GetNotFollowedLogins, _homeHelperFunctions.FormatQueryString(idList));
            }

            return await _homeHelperFunctions.GetUserProfiles(storageConfig, QueryHeaders.AllApprovedTutors, null);
        }

        public async Task<bool> IsFollowingFollowee([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if(followerId != null && followeeId != null)
            {
                var followerAndfollowee = await DataStore.GetListAsync<Follow>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetFollowerAndFollowee.ToString(), new List<string> { followerId, followeeId });
                if (followerAndfollowee.Count > 0) return true;
            }

            return false;
        }

        public async Task<int> GetNumberOfFollowers([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followeeId)
        {
            if(followeeId != null)
            {
                var listOfFollowers = await DataStore.GetListAsync<Follow>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetNumberOfFollowers.ToString(), new List<string> { followeeId });
                return listOfFollowers.Count;
            }

            return 0;
        }
    }
}
