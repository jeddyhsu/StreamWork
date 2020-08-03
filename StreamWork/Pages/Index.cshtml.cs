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
        private readonly StorageService storage;
        private readonly CookieService session;
        private readonly EncryptionService encryption;

        public UserLogin GenericUserProfile { get; set; }
        public UserLogin FeaturedTutor { get; set; }
        public UserChannel FeaturedChannel { get; set; }
        public UserArchivedStreams FeaturedArchivedStream { get; set; }
        public List<UserArchivedStreams> ArchivedStreams { get; set; }
        public bool IsUserFollowingFeaturedTutor { get; set; }
        public string ChatInfo { get; set; }

        public IndexModel(StorageService storage, CookieService session, EncryptionService encryption)
        {
            this.storage = storage;
            this.session = session;
            this.encryption = encryption;
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
            List<UserArchivedStreams> streamsByViews = await storage.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsInDescendingOrderByViews);
            List<UserArchivedStreams> userArchivedStreams = new List<UserArchivedStreams>();

            foreach (string streamWithPriority in streamsWithPriority) // Add hardcoded streams
            {
                int streamIndex = streamsByViews.FindIndex(x => x.StreamID.Equals(streamWithPriority));
                userArchivedStreams.Add(streamsByViews[streamIndex]);
                streamsByViews.RemoveAt(streamIndex);
            }

            int toAdd = 12 - userArchivedStreams.Count; // Since Count changes while the loop is running
            for (int i = 0; i < toAdd; i++) // Fill the rest in with streams in order of view count
            {
                userArchivedStreams.Add(streamsByViews[i]);
            }
            ArchivedStreams = userArchivedStreams;

            UserChannel streamingChannel = await storage.Get<UserChannel>(SQLQueries.GetAllUserChannelsThatAreStreaming);
            if (streamingChannel == null)
            {
                FeaturedChannel = await storage.Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, "juliamkim");
                FeaturedTutor = await storage.Get<UserLogin>(SQLQueries.GetUserWithUsername, "juliamkim");
                FeaturedArchivedStream = await storage.Get<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, "juliamkim");
                FeaturedArchivedStream.StreamSubjectIcon = MiscHelperMethods.GetCorrespondingSubjectThumbnail(FeaturedArchivedStream.StreamSubject);
            }
            else
            {
                FeaturedChannel = streamingChannel;
                FeaturedTutor = await storage.Get<UserLogin>(SQLQueries.GetUserWithUsername, streamingChannel.Username);
            }

            if (session.Authenticated)
            {
                GenericUserProfile = await session.GetCurrentUser();
                ChatInfo = encryption.EncryptString(GenericUserProfile.Username + "|" + GenericUserProfile.Id + "|" + GenericUserProfile.EmailAddress);
            }
        }
    }
}
