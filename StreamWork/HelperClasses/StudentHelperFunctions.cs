using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
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
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                ArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos),
                PreviouslyWatchedStreams = await _homeHelperFunctions.GetPreviouslyWatchedStreams(storageConfig, user),
                LiveChannels = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming)
            };

            model.FollowedTutors = await _followingHelperFunctions.GetFollowedTutors(storageConfig, model.UserProfile);
            model.AllTutors = await _followingHelperFunctions.GetNotFollowedTutors(storageConfig, model.FollowedTutors);
            return model;
        }
    }
}
