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
using StreamWork.ViewModels;

namespace StreamWork.Pages.Tutor
{
    public class TutorDashboard : PageModel
    {
        private readonly SessionService sessionService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly ScheduleService scheduleService;
        private readonly FollowService followService;

        public UserLogin UserProfile { get; set; }
        public UserChannel UserChannel { get; set; }
        public int NumberOfStreams { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfViews { get; set; }
        public List<UserArchivedStreams> UserArchivedStreams { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Schedule> Schedule { get; set; }

        public SearchViewModel SearchViewModel { get; set; }

        public TutorDashboard(StorageService storage, SessionService session, ProfileService profile, ScheduleService schedule, FollowService follow)
        {
            storageService = storage;
            sessionService = session;
            profileService = profile;
            scheduleService = schedule;
            followService = follow;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!sessionService.Authenticated)
            {
                //return Redirect(session.Url("/Home/Login?dest=-Tutor-TutorDashboard"));
            }

            UserProfile = await sessionService.GetCurrentUser();
            UserChannel = await storageService.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, new string[] { UserProfile.Username });

            UserArchivedStreams = await storageService.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, new string[] { UserProfile.Username });
            Sections = profileService.GetSections(UserProfile);
            Topics = profileService.GetTopics(UserProfile);
            //Comments = storage.GetCommentsToTutor(UserProfile.Username);
            Schedule = await scheduleService.GetSchedule(UserProfile.Username);

            NumberOfStreams = UserArchivedStreams.Count;
            NumberOfViews = UserArchivedStreams.Sum(x => x.Views);
            NumberOfFollowers = await followService.GetNumberOfFollowers(UserProfile.Id);

            return Page();
        }
    }
}
