using System;
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
    public class Student : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;

        public UserLogin CurrentUserProfile { get; set; }
        public UserLogin UserProfile { get; set; }
        public List<UserLogin> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Comment> Comments { get; set; }

        public Student(StorageService storage, CookieService cookie, ProfileService profile)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
        }

        public async Task<IActionResult> OnGet(string student)
        {
            if (!cookieService.Authenticated)
            {
                return Redirect(cookieService.Url("/Home/SignIn"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserProfile = await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, student);

            RelatedTutors = (await storageService.GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(UserProfile);
            Topics = profileService.GetTopics(UserProfile);

            return Page();
        }
    }
}
