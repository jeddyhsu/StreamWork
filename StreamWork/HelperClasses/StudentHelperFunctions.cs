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
    public class StudentHelperFunctions ////For functions involved with student code only
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();
        readonly FollowingHelperFunctions _followingHelperFunctions = new FollowingHelperFunctions();

        public async Task<ProfileStudentViewModel> PopulateProfileStudentPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                StudentUserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                ArchivedStreams = await _homeHelperFunctions.GetAllArchivedStreams(storageConfig),
                PreviouslyWatchedStreams = await _homeHelperFunctions.GetPreviouslyWatchedStreams(storageConfig, user),
                LiveChannels = await _homeHelperFunctions.GetAllUserChannels(storageConfig)
            };

            model.FollowedTutors = await _followingHelperFunctions.GetFollowedTutors(storageConfig, model.StudentUserProfile);
            model.AllTutors = await _followingHelperFunctions.GetNotFollowedTutors(storageConfig, model.FollowedTutors);
            return model;
        }

        public async Task DeleteUser([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin user)
        {
            await DataStore.DeleteAsync<UserLogin>(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } });
        }
    }
}
