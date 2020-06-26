using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class FollowService : StorageService
    {
        public FollowService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }
       
        public async Task<bool> AddFollower(string followerId, string followeeId)
        {
            var followerProfile = await Get<UserLogin>(SQLQueries.GetUserWithId, followerId);
            var followeeProfile = await Get<UserLogin>(SQLQueries.GetUserWithId, followeeId);

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

                await Save<Follow>(followRequest.Id, followRequest);

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveFollower(string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if (await Run<Follow>(SQLQueries.DeleteFollower, new string[] { followerId, followeeId })) return true;
            }

            return false;
        }

        public async Task<List<UserLogin>> GetAllFollowees(string followerId)
        {
            var listOfFollowees = await GetList<Follow>(SQLQueries.GetAllFollowersWithId, new string[] { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await GetList<UserLogin>(SQLQueries.GetAllUsersInTheList, new string[] { MiscHelperMethods.FormatQueryString(idList)});
            }

            return null;
        }

        public async Task<List<UserLogin>> GetAllNonFollowees(string followerId) //all users that arent follwing the followee
        {
            var listOfFollowees = await GetList<Follow>(SQLQueries.GetAllFollowersWithId, new string[] { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, MiscHelperMethods.FormatQueryString(idList));
            }

            return await GetList<UserLogin>(SQLQueries.GetAllApprovedTutors, null);
        }

        public async Task<List<UserLogin>> GetAllFollowers(string followeeId) //all users that arent follwing the followee
        {
            var listOfFollowers = await GetList<Follow>(SQLQueries.GetAllFolloweesWithId, new string[] { followeeId });
            if (listOfFollowers.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var follower in listOfFollowers) idList.Add(follower.FollowerId);
                return await GetList<UserLogin>(SQLQueries.GetAllUsersInTheList, new string[] { MiscHelperMethods.FormatQueryString(idList) });
            }

            return null;
        }

        public async Task<bool> IsFollowingFollowee(string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                var followerAndfollowee = await GetList<Follow>(SQLQueries.GetFollowerAndFollowee, new string[] { followerId, followeeId });
                if (followerAndfollowee.Count > 0) return true;
            }

            return false;
        }

        public async Task<int> GetNumberOfFollowers(string followeeId)
        {
            if (followeeId != null)
            {
                var listOfFollowers = await GetList<Follow>(SQLQueries.GetNumberOfFollowers, new string[] { followeeId });
                return listOfFollowers.Count;
            }

            return 0;
        }
    }
}
