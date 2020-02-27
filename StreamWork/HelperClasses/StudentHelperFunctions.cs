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
        readonly FollowingHelperFunctions _followingHelperFunctions = new FollowingHelperFunctions();

        public async Task<ProfileStudentViewModel> PopulateProfileStudentPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            var studentProfile = (await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.CurrentUser, user))[0];

            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                UserLogins = await _homeHelperFunctions.GetUserLogins(storageConfig,QueryHeaders.AllApprovedTutors,null),
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                UserArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos),
                //FollowedTutors = await _followingHelperFunctions.GetFollowedTutors(storageConfig, studentProfile)
            };

            return model;
        }

        public async Task<ProfileStudentViewModel> PopulateArchivePage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string searchQuery, string user)
        {
            searchQuery = searchQuery == null ? "" : searchQuery.ToLower();
            var archive = subject == null ? await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos)
                                          : await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideosBasedOnSubject, subject);

            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                UserArchivedStreams = (from a in archive select a).Where(a => a.StreamTitle.ToLower().Contains(searchQuery)).ToList(),
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
            };

            return model;
        }

        public async Task<ProfileStudentViewModel> PopulateStudentLiveStreamsPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string subject, string searchQuery, string user)
        {
            searchQuery = searchQuery == null ? "" : searchQuery.ToLower();
            var streams = subject == null ? await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreaming, "")
                                          : await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.AllUserChannelsThatAreStreamingWithSpecifiedSubject, subject);

            ProfileStudentViewModel model = new ProfileStudentViewModel
            {
                UserChannels = (from s in streams select s).Where(s => s.StreamTitle.ToLower().Contains(searchQuery)).ToList(),
                UserProfile = user == null ? null : await _homeHelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user),
                UserArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.AllArchivedVideos),
            };

            return model;
        }
    }
}
