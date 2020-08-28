using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Profiles
{
    public class Student : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly NotificationService notificationService;
        private readonly EncryptionService encryptionService;

        public Profile CurrentUserProfile { get; set; }
        public Profile UserProfile { get; set; }
        public List<Profile> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public Student(StorageService storage, CookieService cookie, ProfileService profile, NotificationService notification, EncryptionService encryption)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            notificationService = notification;
            encryptionService = encryption;
        }

        public async Task<IActionResult> OnGet(string student)
        {
            if (!await cookieService.ValidateUserType(student, "student")) //checks for 
            {
                return Redirect("/Profiles/Tutor/" + student);
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserProfile = await storageService.Get<Profile>(SQLQueries.GetUserWithUsername, student);

            RelatedTutors = (await storageService.GetList<Profile>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(UserProfile);
            Topics = profileService.GetTopics(UserProfile);

            if (CurrentUserProfile != null)
            {
                Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
                AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            }

            return Page();
        }
    }
}
