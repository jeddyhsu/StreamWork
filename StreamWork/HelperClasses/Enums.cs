﻿
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
