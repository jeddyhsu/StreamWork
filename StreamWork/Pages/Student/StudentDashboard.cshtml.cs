﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;
using StreamWork.ProfileObjects;

namespace StreamWork.Pages.Student
{
    public class StudentDashboard : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly ProfileService profileService;
        private readonly NotificationService notificationService;
        private readonly FollowService followService;
        private readonly ScheduleService scheduleService;

        public Profile CurrentUserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public List<Profile> RelatedTutors { get; set; }
        public List<FollowedTutors> FollowedTutors { get; set; }
        public List<Section> Sections { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Comment> Comments { get; set; }
        public List<string> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public string DefaultBanner { get; set; }
        public string DefaultProfilePicture { get; set; }

        public StudentDashboard(StorageService storage, CookieService cookie, ProfileService profile, NotificationService notification, FollowService follow, ScheduleService schedule)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            notificationService = notification;
            followService = follow;
            scheduleService = schedule;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!cookieService.Authenticated || (await cookieService.GetCurrentUser()).ProfileType != "student")
            {
                return Redirect(cookieService.Url("/Home/SignIn/SW"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();

            RelatedTutors = (await storageService.GetList<Profile>(SQLQueries.GetAllTutorsNotInTheList, new string[] { CurrentUserProfile.Id })).GetRange(0, 3);
            FollowedTutors = await GetFollowedTutors(CurrentUserProfile.Id);
            Sections = profileService.GetSections(CurrentUserProfile);
            Topics = profileService.GetTopics(CurrentUserProfile);

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            DefaultBanner = MiscHelperMethods.defaultBanner;
            DefaultProfilePicture = MiscHelperMethods.defaultProfilePicture;

            return Page();
        }

        private async Task<List<FollowedTutors>> GetFollowedTutors(string followeeId)
        {
            List<FollowedTutors> followedTutorsList = new List<FollowedTutors>();
            var followedTutors = await followService.GetAllFollowees(followeeId);

            if(followedTutors != null && followedTutors.Count > 0)
            {
                foreach (var tutor in followedTutors)
                {
                    var previousStreams = (await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, tutor.Username)).Count >= 3 ? (await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, tutor.Username)).GetRange(0, 3) : (await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, tutor.Username));
                    var latestScheduledStream = (await scheduleService.GetSchedule(tutor)).Count == 0 ? null : (await scheduleService.GetSchedule(tutor))[0];
                    var followValue = await followService.IsFollowingFollowee(CurrentUserProfile.Id, tutor.Id);

                    followedTutorsList.Add(new FollowedTutors(tutor, previousStreams, latestScheduledStream, followValue));
                }
            }

            return followedTutorsList;
        }
    }
}
