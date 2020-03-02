using System.Linq;
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

        public async Task<ProfileStudentViewModel> PopulateProfileStudentPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                UserLogins = await _homeHelperFunctions.GetUserLogins(storageConfig,QueryHeaders.AllApprovedTutors,null),
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                UserArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos),
                //FollowedTutors = await _followingHelperFunctions.GetFollowedTutors(storageConfig, studentProfile)
            };

            return model;
        }
    }
}
