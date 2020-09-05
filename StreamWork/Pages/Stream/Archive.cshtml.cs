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
        private readonly EncryptionService encryptionService;

        public Profile CurrentUserProfile { get; set; }
        public Profile UserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public Video Video { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public string FollowValue { get; set; }
        public List<Video> UserVideos { get; set; }
        public List<Video> OtherVideos { get; set; }
        public List<Profile> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Schedule> Schedule { get; set; }
        public List<Comment> Comments { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<string> Notifications { get; set; }
        public Comment NotificationRequestComment { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public Archive(StorageService storage, CookieService cookie, ProfileService profile, ScheduleService schedule, FollowService follow, CommentService comment, NotificationService notification, EncryptionService encryption)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            commentService = comment;
            notificationService = notification;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string id, string commentId)
        {
            if (!cookieService.Authenticated)
            {
                return Redirect(cookieService.Url("/Home/SignIn/" + encryptionService.EncryptString("/Stream/Archive/" + id + "/" + commentId)));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            Video = await storageService.Get<Video>(SQLQueries.GetArchivedStreamsWithStreamId, id);
            UserProfile = await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, Video.Username);
            UserChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, Video.Username);
            UserChannel.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(Video.StreamSubject);
            ChatInfo = encryptionService.EncryptString(Video.Id);
            FollowValue = await followService.IsFollowingFollowee(CurrentUserProfile.Id, UserProfile.Id);

            UserVideos = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { UserProfile.Username });
            OtherVideos = await storageService.GetList<Video>(SQLQueries.GetRandomArchivedStreams, new string[] { });
            RelatedTutors = (await storageService.GetList<Profile>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(UserProfile);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);
            Comments = await commentService.GetAllComments(Video.Id);

            NumberOfStreams = UserVideos.Count;
            NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);
            NumberOfViews = UserVideos.Sum(x => x.Views);

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            if (!string.IsNullOrEmpty(commentId)) NotificationRequestComment = await storageService.Get<Comment>(SQLQueries.GetCommentWithId, commentId);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);

            return Page();
        }
    }
}
