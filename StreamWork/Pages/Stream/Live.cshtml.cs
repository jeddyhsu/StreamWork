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
        private readonly EditService editService;
        private readonly ChatService chatService;
        private readonly NotificationService notificationService;

        public UserLogin UserProfile { get; set; }
        public UserLogin CurrentUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public string FollowValue { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<UserLogin> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Schedule> Schedule { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public Live(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow, EditService edit, ChatService chat, NotificationService notification)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            editService = edit;
            chatService = chat;
            notificationService = notification;
        }

        public async Task<IActionResult> OnGet(string tutor)
        {
            if (!cookieService.Authenticated)
            {
                return Redirect(cookieService.Url("/Home/SignIn"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserProfile = await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, tutor);
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, tutor);
            ChatInfo = "1234";
            FollowValue = await followService.IsFollowingFollowee(UserProfile.Id, CurrentUserProfile.Id);

            UserArchivedStreams = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { CurrentUserProfile.Username });
            RelatedTutors = (await storageService.GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, new string[] { CurrentUserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(CurrentUserProfile);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);

            Notifications = await notificationService.GetNotifications(UserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(UserProfile.Username);

            return Page();
        }
    }
}
