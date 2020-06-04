
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
        UsernameExists,
        EmailExists,
        PayPalEmailExists
    }

    public enum DatabaseValues
    {
        True,
        False,
    }

    public enum QueryHeaders
    {
        // UserLogin
        AllSignedUpUsers,
        CurrentUser,
        CurrentUserUsingEmail,
        CheckUserUsingPayPalAddress,
        AllSignedUpUsersWithPassword,
        AllApprovedTutors,
        ApprovedTutorsByFollowers,
        AllStudents,
        GetFollowedLogins,
        GetNotFollowedLogins,
        CurrentUserUsingId,

        // UserChannel
        AllUserChannelsThatAreStreamingWithSpecifiedSubject,
        AllUserChannelsThatAreStreaming,
        CurrentUserChannel,
        CurrentUserChannelFromId,
        UserChannelsBySearchTerm,
        UserChannelsBySubjectAndSearchTerm,

        // UserArchivedStream
        AllArchivedVideos,
        ArchivedVideosById,
        UserArchivedVideos,
        UserArchivedVideosBasedOnSubject,
        ArchivedVideosByStreamId,
        ArchivedVideosBySearchTerm,
        ArchivedVideosBySubjectAndSearchTerm,
        MultipleArchivedVideosByStreamId,
        LatestArchivedStreamByUser,
        LatestArchivedStreams,
        ArchivedStreamsByViews,
        ArchivedStreamsDescendingOrder,

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

        //Chats
        GetAllChats,
        GetAllChatsFromSpecificId,
        DeleteAllChatsFromSpecificId,

        //Follow
        GetAllFollowersWithSpecificId,
        GetAllFolloweesWithSpecificId,
        GetFollowerAndFollowee,
        GetNumberOfFollowers,
        RemoveFollower,

        // Misc
        UserProfile,       //Use For Session
    }
}
