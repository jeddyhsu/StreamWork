
namespace StreamWork.HelperClasses
{
    public class Enums { }

    public enum JsonResponse
    {
        Success,
        Failed,
        QueryFailed,
        Tutor,
        Student,
    }

    public enum QueryHeaders
    {
        // UserLogin
        AllSignedUpUsers,
        CurrentUser,
        CheckUsersWithSamePayPalAddress,
        AllSignedUpUsersWithPassword,
        AllApprovedTutors,
        AllStudents,

        // UserChannel
        AllUserChannelsThatAreStreamingWithSpecifiedSubject,
        AllUserChannelsThatAreStreaming,
        CurrentUserChannel,
        CurrentUserChannelFromId,
        UserChannelsBySearchTerm,
        UserChannelsBySubjectAndSearchTerm,

        // UserArchivedStream
        AllArchivedVideos,
        UserArchivedVideos,
        UserArchivedVideosBasedOnSubject,
        ArchivedVideosByStreamId,
        ArchivedVideosBySearchTerm,
        ArchivedVideosBySubjectAndSearchTerm,

        // Payment
        AllPayments,
        PaymentsById,

        // Recommendation
        RecommendationsByTutor,

        // View
        ViewsByViewer,
        ViewsByStreamId,
        ViewsByViewerAndStreamId,
        ViewsByChannelSince,
        ViewsByViewerAndChannelSince,

        // Misc
        UserProfile,       //Use For Session
    }
}
