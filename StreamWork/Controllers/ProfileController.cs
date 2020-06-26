using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.HelperMethods;
using StreamWork.ViewModels.Profile;

namespace StreamWork.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly TutorMethods _tutorMethods = new TutorMethods();
        private readonly FollowingMethods _followingMethods = new FollowingMethods();
        private readonly ScheduleMethods _scheduleMethods = new ScheduleMethods();

        public async Task<IActionResult> Tutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string tutor)
        {
            var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, tutor);

            TutorProfileViewModel viewModel = new TutorProfileViewModel
            {
                UserProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, User.Identity.Name),
                UserChannel = await _homeMethods.GetUserChannel(storageConfig, SQLQueries.GetUserChannelWithUsername, User.Identity.Name),
                UserArchivedStreams = await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name),
                LatestStream = await _homeMethods.GetArchivedStream(storageConfig, SQLQueries.GetLatestArchivedStreamByUser, tutor),
                NumberOfStreams = (await _homeMethods.GetArchivedStreams(storageConfig, SQLQueries.GetArchivedStreamsWithUsername, User.Identity.Name)).Count,
                Sections = _tutorMethods.GetSections(userProfile),
                Topics = _tutorMethods.GetTopics(userProfile),
                Comments = await _homeMethods.GetCommentsForTutor(storageConfig, User.Identity.Name),
            };

            int viewCount = 0;
            foreach (var archivedStream in viewModel.UserArchivedStreams)
            {
                viewCount += archivedStream.Views;
            }

            viewModel.NumberOfViews = viewCount;
            viewModel.NumberOfFollowers = await _followingMethods.GetNumberOfFollowers(storageConfig, viewModel.UserProfile.Id);
            viewModel.Schedule = await _scheduleMethods.GetSchedule(storageConfig, tutor);

            return View(viewModel);
        }
    }
}
