﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.TutorObjects;

namespace StreamWork.Pages.Profile
{
    public class Student : PageModel
    {
        private readonly SessionService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;

        public UserLogin CurrentUserProfile { get; set; }
        public UserLogin UserProfile { get; set; }
        public List<UserLogin> RelatedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<DataModels.Comment> Comments { get; set; }

        public Student(StorageService storage, SessionService session, ProfileService profile, ScheduleService schedule, FollowService follow)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
        }

        public async Task<IActionResult> OnGet(string student)
        {
            if (!sessionService.Authenticated)
            {
                //return Redirect(session.Url("/Home/Login?dest=-Tutor-TutorDashboard"));
            }

            CurrentUserProfile = await sessionService.GetCurrentUser();
            UserProfile = await storageService.Get<UserLogin>(SQLQueries.GetUserWithUsername, student);

            RelatedTutors = (await storageService.GetList<UserLogin>(SQLQueries.GetAllTutorsNotInTheList, new string[] { UserProfile.Id })).GetRange(0, 5);
            Sections = profileService.GetSections(UserProfile);
            Topics = profileService.GetTopics(UserProfile);

            return Page();
        }
    }
}
