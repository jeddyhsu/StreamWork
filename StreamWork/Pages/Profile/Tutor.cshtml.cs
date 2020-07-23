using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Profile
{
    public class Tutor : PageModel
    {
        private readonly SessionService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly NotificationService notificationService;

        public UserLogin CurrentUserProfile { get; set; }
        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public UserArchivedStreams LatestStream { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<UserLogin> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Schedule> Schedule { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public Tutor(StorageService storage, SessionService session, ProfileService profile, ScheduleService schedule, FollowService follow, NotificationService notification)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            notificationService = notification;
        }

        public async Task<IActionResult> OnGet(string tutor)
        {
            if (!sessionService.Authenticated)
            {
                //return Redirect(session.Url("/Home/Login?dest=-Tutor-TutorDashboard"));
            }

            CurrentUserProfile = await sessionService.GetCurrentUser();
            UserProfile = await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, tutor);
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, new string[] { UserProfile.Username });

            LatestStream = await storageService.Get<UserArchivedStreams>(SQLQueries.GetLatestArchivedStreamByUser, new string[] { UserProfile.Username });
            UserArchivedStreams = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { UserProfile.Username });
            RelatedTutors = (await storageService.GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0,5);
            Sections = profileService.GetSections(UserProfile);
            Topics = profileService.GetTopics(UserProfile);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);
            NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);

            Notifications = await notificationService.GetNotifications(UserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(UserProfile.Username);

            return Page();
        }
    }
}
