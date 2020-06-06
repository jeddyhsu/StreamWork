using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.ViewModels;

namespace StreamWork.HelperClasses
{
    public class StudentMethods ////For functions involved with student code only
    {
        readonly HomeMethods _homeHelperFunctions = new HomeMethods();
        readonly FollowingMethods _followingHelperFunctions = new FollowingMethods();

        public async Task<ProfileStudentViewModel> PopulateProfileStudentPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                StudentUserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user),
                ArchivedStreams = await _homeHelperFunctions.GetAllArchivedStreams(storageConfig),
                PreviouslyWatchedStreams = await _homeHelperFunctions.GetPreviouslyWatchedStreams(storageConfig, user),
                LiveChannels = await _homeHelperFunctions.GetAllUserChannels(storageConfig)
            };

            model.FollowedTutors = await _followingHelperFunctions.GetAllFollowees(storageConfig, model.StudentUserProfile.Id);
            model.NonFollowedTutors = await _followingHelperFunctions.GetAllNonFollowees(storageConfig, model.StudentUserProfile.Id);
            return model;
        }

        public async Task DeleteUser([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin user)
        {
            await DataStore.DeleteAsync<UserLogin>(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } });
        }
    }
}
