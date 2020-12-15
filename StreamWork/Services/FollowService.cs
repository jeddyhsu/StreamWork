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
        NotificationService notificationService; //only this service is allowed in other service classes

        public FollowService([FromServices] IOptionsSnapshot<StorageConfig> config, NotificationService notification) : base(config) { notificationService = notification; }

        public async Task<bool> AddFollower(string followerId, string followeeId)
        {
            var followerProfile = await Get<Profiles>(SQLQueries.GetUserWithId, followerId);
            var followeeProfile = await Get<Profiles>(SQLQueries.GetUserWithId, followeeId);

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

                await Save(followRequest.Id, followRequest);

                await notificationService.SaveNotification(NotificationType.Follow, followerProfile.Username, followeeProfile.Username, followRequest.Id);
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

        public async Task<List<Profiles>> GetAllFollowers(string followeeId) //all users that arent follwing the followee
        {
            var listOfFollowers = await GetList<Follow>(SQLQueries.GetAllFollowersWithId, new string[] { followeeId });
            if (listOfFollowers.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var follower in listOfFollowers) idList.Add(follower.FollowerId);
                return await GetList<Profiles>(SQLQueries.GetAllUsersInTheList, new string[] { MiscHelperMethods.FormatQueryString(idList) });
            }

            return null;
        }

        public async Task<List<Profiles>> GetAllFollowees(string followerId)
        {
            var listOfFollowees = await GetList<Follow>(SQLQueries.GetAllFolloweesWithId, new string[] { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await GetList<Profiles>(SQLQueries.GetAllUsersInTheList, new string[] { MiscHelperMethods.FormatQueryString(idList) });
            }

            return null;
        }

        public async Task<List<Profiles>> GetAllNonFollowees(string followerId) //all users that arent follwing the followee
        {
            var listOfFollowees = await GetList<Follow>(SQLQueries.GetAllFollowersWithId, new string[] { followerId });
            if (listOfFollowees.Count != 0)
            {
                List<string> idList = new List<string>();
                foreach (var followee in listOfFollowees) idList.Add(followee.FolloweeId);
                return await GetList<Profiles>(SQLQueries.GetAllTutorsNotInTheList, MiscHelperMethods.FormatQueryString(idList));
            }

            return await GetList<Profiles>(SQLQueries.GetAllApprovedTutors, null);
        }

        public async Task<string> IsFollowingFollowee(string followerId, string followeeId)
        {
            if (followerId != null && followeeId != null)
            {
                if (followeeId == followerId) return FollowValues.NoFollow.ToString();
                var followerAndfollowee = await GetList<Follow>(SQLQueries.GetFollowerAndFollowee, new string[] { followerId, followeeId });
                if (followerAndfollowee.Count > 0) return FollowValues.Following.ToString();
            }

            return FollowValues.Follow.ToString();
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
