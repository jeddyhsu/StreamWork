using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Profiles
{
    public class Tutor : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly NotificationService notificationService;
        private readonly EncryptionService encryptionService;

        public Profile CurrentUserProfile { get; set; }
        public Profile UserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public Video LatestStream { get; set; }
        public List<Video> UserArchivedStreams { get; set; }
        public List<Profile> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<ScheduledStream> Schedule { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<string> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public string FollowValue { get; set; }

        public Tutor(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow, NotificationService notification, EncryptionService encryption)
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
            //if (!await cookieService.ValidateUserType(tutor, "tutor")) //checks for 
            //{
            //    return Redirect("/Profiles/Student/" + tutor);
            //}

            //CurrentUserProfile = await cookieService.GetCurrentUser();
            //UserProfile = await storageService.Get<DataModels.Profiles>(SQLQueries.GetUserWithUsername, tutor);
            //UserChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, new string[] { UserProfile.Username });

            //LatestStream = await storageService.Get<Video>(SQLQueries.GetLatestArchivedStreamByUser, new string[] { UserProfile.Username });
            //UserArchivedStreams = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { UserProfile.Username });
            //RelatedTutors = (await storageService.GetList<DataModels.Profiles>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0, 5);
            //Sections = profileService.GetSections(UserProfile);
            //Topics = profileService.GetTopics(UserProfile);
            //Schedule = await scheduleService.GetSchedule(UserProfile);

            //NumberOfStreams = UserArchivedStreams.Count;
            //NumberOfViews = UserArchivedStreams.Sum(x => x.Views);
            //NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);

            //if (CurrentUserProfile != null)
            //{
            //    Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            //    AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);

            //    FollowValue = await followService.IsFollowingFollowee(CurrentUserProfile.Id, UserProfile.Id);
            //}

            return Page();
        }
    }
}
