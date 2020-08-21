using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Stream
{
    public class Live : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly NotificationService notificationService;
        private readonly EncryptionService encryptionService;

        public Profile UserProfile { get; set; }
        public Profile CurrentUserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public string FollowValue { get; set; }
        public List<Video> UserArchivedStreams { get; set; }
        public List<Profile> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Schedule> Schedule { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public Live(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow, NotificationService notification, EncryptionService encryption)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            notificationService = notification;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string tutor)
        {
            if (!cookieService.Authenticated)
            {
                return Redirect(cookieService.Url("/Home/SignIn/" + encryptionService.EncryptString("/Stream/Live/" + tutor)));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserProfile = await storageService.Get<DataModels.Profile>(SQLQueries.GetUserWithUsername, tutor);
            UserChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, tutor);

            if(UserChannel.StreamTitle == null)
            {
                return Redirect(cookieService.Url("/Profiles/Tutor/" + UserProfile.Username));
            }

            UserChannel.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(UserChannel.StreamSubject);
            ChatInfo = encryptionService.EncryptString(UserChannel.ArchivedVideoId);
            FollowValue = await followService.IsFollowingFollowee(CurrentUserProfile.Id, UserProfile.Id);

            UserArchivedStreams = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { UserProfile.Username });
            RelatedTutors = (await storageService.GetList<DataModels.Profile>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(UserProfile);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);

            return Page();
        }
    }
}
