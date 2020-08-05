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

        public Profile CurrentUserProfile { get; set; }
        public Profile FeaturedTutor { get; set; }
        public Channel FeaturedChannel { get; set; }
        public Video FeaturedArchivedStream { get; set; }
        public List<Video> ArchivedStreams { get; set; }
        public bool IsUserFollowingFeaturedTutor { get; set; }
        public string ChatInfo { get; set; }

        public IndexModel(StorageService storage, CookieService cookie, EncryptionService encryption)
        {
            storageService = storage;
            cookieService = cookie;
            encryptionService = encryption;
        }

        public async Task OnGet()
        {
            // List of streams for the carousel
            List<string> streamsWithPriority = new List<string> {
                "F8U3mEscyNB_1",
                "EBRNrFsAqZZ_1",
                "E1OKuVsAi9U_1",
                "EdowSgsAqJV_1",
                "EYd2jUscrUz_1",
                "Fr40wrscyQF_1",
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
            ArchivedStreams = videos;

            Channel streamingChannel = await storageService.Get<Channel>(SQLQueries.GetAllUserChannelsThatAreStreaming);
            if (streamingChannel == null)
            {
                FeaturedChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, "juliamkim");
                FeaturedTutor = await storageService.Get<DataModels.Profile>(SQLQueries.GetUserWithUsername, "juliamkim");
                FeaturedArchivedStream = await storageService.Get<Video>(SQLQueries.GetArchivedStreamsWithUsername, "juliamkim");
                FeaturedArchivedStream.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(FeaturedArchivedStream.StreamSubject);
            }
            else
            {
                FeaturedChannel = streamingChannel;
                FeaturedTutor = await storageService.Get<DataModels.Profile>(SQLQueries.GetUserWithUsername, streamingChannel.Username);
                FeaturedChannel.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(FeaturedChannel.StreamSubject);
            }

            if (cookieService.Authenticated)
            {
                CurrentUserProfile = await cookieService.GetCurrentUser();
                ChatInfo = encryptionService.EncryptString(CurrentUserProfile.Username + "|" + CurrentUserProfile.Id + "|" + CurrentUserProfile.EmailAddress);
            }
        }
    }
}
