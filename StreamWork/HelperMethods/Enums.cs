
namespace StreamWork.HelperMethods
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
        PayPalEmailExists,
        WrongUsernameOrPassword
    }

    public enum DatabaseValues
    {
        True,
        False,
    }

    public enum SQLQueries
    {
        // UserLogin
        GetAllUsers,
        GetUserWithUsername,
        GetUserWithEmailAddress,
        GetUserWithPayPalAddress,
        GetUserUsingId,
        GetUserWithUsernameAndPassword,
        GetAllApprovedTutors,
        GetApprovedTutorsByFollowers,
        GetAllStudents,
        GetAllUsersInTheList,
        GetAllTutorsNotInTheList,
        
        // UserChannel
        GetUserChannelWithSubject,
        GetAllUserChannelsThatAreStreaming,
        GetUserChannelWithUsername,
        GetUserChannelWithId,
        GetUserChannelsBySearchTerm,
        GetUserChannelsBySubjectAndSearchTerm,

        // UserArchivedStream
        GetAllArchivedStreams,
        GetArchivedStreamsWithId,
        GetArchivedStreamsWithUsername,
        GetArchivedStreamsWithSubject,
        GetArchivedStreamsWithStreamId,
        GetArchivedStreamsWithSearchTerm,
        GetArchivedStreamsWithSubjectAndSearchTerm,
        GetArchivedStreamsByStreamIdInTheList,
        GetLatestArchivedStreamByUser,
        GetArchivedStreamsInDescendingOrderByDate,
        GetArchivedStreamsInDescendingOrderByViews,

        // Payment
        GetAllPayments,
        GetPaymentsById,

        // Recommendation
        GetRecommendationsWithTutorUsername,

        // View
        GetViewsWithViewer,
        GetViewsWithStreamId,
        GetViewsWithViewerAndStreamId,
        GetViewsWithChannelSince,
        GetViewsWithViewerAndChannelSince,

        //Chats
        GetAllChats,
        GetAllChatsWithId,
        DeleteAllChatsWithId,

        //Follow
        GetAllFolloweesWithId,
        GetAllFollowersWithId,
        GetFollowerAndFollowee,
        GetNumberOfFollowers,
        DeleteFollower,

        //Schedule
        GetScheduleWithUserUsername,
        GetScheduleWithId,
        DeleteScheduleWithId,

        // Misc
        UserProfile,       //Use For Session
    }
}
