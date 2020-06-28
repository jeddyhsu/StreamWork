using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Stream
{
    public class Live : PageModel
    {
        private readonly SessionService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;
        private readonly EditService editService;
        private readonly ChatService chatService;

        public UserLogin UserProfile { get; set; }
        public UserLogin TutorUserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public string StreamSubjectPicture { get; set; }
        public bool IsFollowing { get; set; }

        public Live(StorageService storage, SessionService session, ProfileService profile, ScheduleService schedule, FollowService follow, EditService edit, ChatService chat)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
            editService = edit;
            chat = chatService;
        }

        public async Task<IActionResult> OnGet(string tutor)
        {
            UserProfile = await sessionService.GetCurrentUser();
            TutorUserProfile = await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, tutor);
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, tutor);
            ChatInfo = "1234";
            IsFollowing = await followService.IsFollowingFollowee(UserProfile.Id, TutorUserProfile.Id);

            return Page();
        }
    }
}
