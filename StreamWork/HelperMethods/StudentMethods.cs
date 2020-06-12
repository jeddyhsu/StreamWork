using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.ViewModels.Student;

namespace StreamWork.HelperMethods
{
    public class StudentMethods ////For functions involved with student code only
    {
        readonly HomeMethods _homeMethods = new HomeMethods();
        readonly FollowingMethods _followingMethods = new FollowingMethods();

        public async Task<ProfileStudentViewModel> PopulateProfileStudentPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                StudentUserProfile = user == null ? null : await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user),
                ArchivedStreams = await _homeMethods.GetAllArchivedStreams(storageConfig),
                PreviouslyWatchedStreams = await _homeMethods.GetPreviouslyWatchedStreams(storageConfig, user),
                LiveChannels = await _homeMethods.GetAllUserChannels(storageConfig)
            };

            model.FollowedTutors = await _followingMethods.GetAllFollowees(storageConfig, model.StudentUserProfile.Id);
            model.NonFollowedTutors = await _followingMethods.GetAllNonFollowees(storageConfig, model.StudentUserProfile.Id);
            return model;
        }

        public async Task DeleteUser([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserLogin user)
        {
            await DataStore.DeleteAsync<UserLogin>(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", user.Id } });
        }
    }
}
