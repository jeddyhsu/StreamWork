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
    public class FollowingMethods //All helper functions that have to with students following tutors
    {
        readonly HomeMethods _homeMethods = new HomeMethods();

        public async Task<bool> AddFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            var followerProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserUsingId, followerId);
            var followeeProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserUsingId, followeeId);

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

                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", followRequest.Id } }, followRequest);

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveFollower([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if (await DataStore.RunQueryAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.DeleteFollower.ToString(), new List<string> { followerId, followeeId })) return true;
            }

            return false;
        }

        public async Task<List<UserLogin>> GetAllFollowees([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId)
        {
            var listOfFollowees = await DataStore.GetListAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetAllFollowersWithId.ToString(), new List<string> { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetAllUsersInTheList, _homeMethods.FormatQueryString(idList));
            }

            return null;
        }

        public async Task<List<UserLogin>> GetAllNonFollowees([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId) //all users that arent follwing the followee
        {
            var listOfFollowees = await DataStore.GetListAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetAllFollowersWithId.ToString(), new List<string> { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetAllTutorsNotInTheList, _homeMethods.FormatQueryString(idList));
            }

            return await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetAllApprovedTutors, null);
        }

        public async Task<List<UserLogin>> GetAllFollowers([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followeeId) //all users that arent follwing the followee
        {
            var listOfFollowers = await DataStore.GetListAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetAllFolloweesWithId.ToString(), new List<string> { followeeId });
            if (listOfFollowers.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var follower in listOfFollowers) idList.Add(follower.FollowerId);
                return await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetAllUsersInTheList, _homeMethods.FormatQueryString(idList));
            }

            return null;
        }

        public async Task<bool> IsFollowingFollowee([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                var followerAndfollowee = await DataStore.GetListAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetFollowerAndFollowee.ToString(), new List<string> { followerId, followeeId });
                if (followerAndfollowee.Count > 0) return true;
            }

            return false;
        }

        //public async Task<List<UserLogin>> GetFolloweesWithMostFollowers([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        //{
        //    var listOfFollowers = await DataStore.GetListAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetFolloweesBasedOnMostFollowers.ToString());
        //    if (listOfFollowers.Count != 0)
        //    {
        //        List<string> idList = new List<string>();
        //        foreach (var follower in listOfFollowers) idList.Add(follower.FollowerId);
        //        return await _homeMethods.GetUserProfiles(storageConfig, SQLQueries.GetAllUsersInTheList, _homeMethods.FormatQueryString(idList));
        //    }

        //    return null;
        //}

        public async Task<int> GetNumberOfFollowers([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string followeeId)
        {
            if (followeeId != null)
            {
                var listOfFollowers = await DataStore.GetListAsync<Follow>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetNumberOfFollowers.ToString(), new List<string> { followeeId });
                return listOfFollowers.Count;
            }

            return 0;
        }
    }
}
