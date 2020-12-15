using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StorageService storageService;
        private readonly CookieService cookieService;
        private readonly EncryptionService encryptionService;
        private readonly NotificationService notificationService;

        public DataModels.Profiles CurrentUserProfile { get; set; }
        public DataModels.Profiles FeaturedTutor { get; set; }
        public Channel FeaturedChannel { get; set; }
        public Video FeaturedArchivedVideo { get; set; }
        public List<Video> ArchivedVideos { get; set; }
        public bool IsUserFollowingFeaturedTutor { get; set; }
        public string ChatInfo { get; set; }
        public List<string> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }

        public IndexModel(StorageService storage, CookieService cookie, EncryptionService encryption, NotificationService notification)
        {
            storageService = storage;
            cookieService = cookie;
            encryptionService = encryption;
            notificationService = notification;
        }

        public async Task OnGet()
        {


            Debug d = new Debug();
            d.Id = Guid.NewGuid().ToString();
            d.Message = "TEST";
            await storageService.Save(d.Id, d, "df");



            await storageService.Get<Debug>("f2fbab1d-011e-4239-8d26-91b25a5341f6");


            var result = await storageService.GetList<Debug>("GetAllDebugs", null);




            // List of streams for the carousel
            List<string> streamsWithPriority = new List<string> {
                "F8U3mEscyNB_1",
                "EBRNrFsAqZZ_1",
                "E1OKuVsAi9U_1",
                "EdowSgsAqJV_1",
                "EYd2jUscrUz_1",
                "6SAvwnsAobr_1",
                "ETWYvVscngb_1",
                "F5pYLrscQ5Q_1"
            };

            // List of the IDs of the streams to hardcode in
            List<Video> videosByViews = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsInDescendingOrderByViews);
            List<Video> videos = new List<Video>();

            foreach (string streamWithPriority in streamsWithPriority) // Add hardcoded streams
            {
                int streamIndex = videosByViews.FindIndex(x => x.StreamID.Equals(streamWithPriority));
                videos.Add(videosByViews[streamIndex]);
                videosByViews.RemoveAt(streamIndex);
            }

            int toAdd = 12 - videos.Count; // Since Count changes while the loop is running
            for (int i = 0; i < toAdd; i++) // Fill the rest in with streams in order of view count
            {
                videos.Add(videosByViews[i]);
            }

            ArchivedVideos = videos;

            Channel streamingChannel = await storageService.Get<Channel>(SQLQueries.GetAllUserChannelsThatAreStreaming);
            if (streamingChannel == null)
            {
                FeaturedChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, "juliamkim");
                FeaturedTutor = await storageService.Get<DataModels.Profiles>(SQLQueries.GetUserWithUsername, "juliamkim");
                FeaturedArchivedVideo = await storageService.Get<Video>(SQLQueries.GetArchivedStreamsWithUsername, "juliamkim");
                FeaturedArchivedVideo.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(FeaturedArchivedVideo.StreamSubject);
            }
            else
            {
                FeaturedChannel = streamingChannel;
                FeaturedTutor = await storageService.Get<DataModels.Profiles>(SQLQueries.GetUserWithUsername, streamingChannel.Username);
                FeaturedChannel.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(FeaturedChannel.StreamSubject);
            }

            if (cookieService.Authenticated)
            {
                CurrentUserProfile = await cookieService.GetCurrentUser();
                Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
                AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);
            }

            ChatInfo = encryptionService.EncryptString(streamingChannel != null ? streamingChannel.ArchivedVideoId : FeaturedArchivedVideo.Id);
        }
    }
}
