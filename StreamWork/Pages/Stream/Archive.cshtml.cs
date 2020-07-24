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
    public class Archive : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly CommentService commentService;
        private readonly NotificationService notificationService;

        public UserLogin CurrentUserProfile { get; set; }
        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public UserArchivedStreams ArchivedStream { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public string FollowValue { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<UserArchivedStreams> OtherArchivedStreams { get; set; }
        public List<UserLogin> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Schedule> Schedule { get; set; }
        public List<Comment> Comments { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<Notification> Notifications { get; set; }
        public Comment NotificationRequestComment { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public Archive(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow, CommentService comment, NotificationService notification)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            commentService = comment;
            notificationService = notification;
        }

        public async Task<IActionResult> OnGet(string tutor, string id, string commentId)
        {
            if (!cookieService.Authenticated)
            {
                return Redirect(cookieService.Url("/Home/SignIn"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserProfile = await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, tutor);
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, tutor);
            ArchivedStream = await storageService.Get<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithStreamId, id);
            ChatInfo = "1234";
            FollowValue = await followService.IsFollowingFollowee(UserProfile.Id, CurrentUserProfile.Id);

            UserArchivedStreams = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { CurrentUserProfile.Username });
            OtherArchivedStreams = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetAllArchivedStreams, new string[] { });
            RelatedTutors = (await storageService.GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, new string[] { CurrentUserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(CurrentUserProfile);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);
            Comments = await commentService.GetAllComments(ArchivedStream.StreamID);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);

            Notifications = await notificationService.GetNotifications(UserProfile.Username);
            if (!string.IsNullOrEmpty(commentId)) NotificationRequestComment = await storageService.Get<Comment>(SQLQueries.GetCommentWithId, commentId);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(UserProfile.Username);

            return Page();
        }
    }
}
