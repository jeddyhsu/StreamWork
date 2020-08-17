
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

    public enum FollowValues
    {
        Follow,
        Following,
        NoFollow,
    }

    public enum NotificationType
    {
        Follow,
        Comment,
        Reply,
    }

    public enum SQLQueries
    {
        // UserLogin
        GetAllUsers,
        GetUserWithUsername,
        GetUserWithEmailAddress,
        GetUserWithPayPalAddress,
        GetUserWithId,
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
        GetArchivedStreamsWithSubjectAndUsername,
        GetArchivedStreamsWithStreamId,
        GetArchivedStreamsWithSearchTerm,
        GetArchivedStreamsWithSearchTermAndUsername,
        GetArchivedStreamsWithSubjectAndSearchTermAndUsername,
        GetArchivedStreamsWithSubjectAndSearchTerm,
        GetArchivedStreamsByStreamIdInTheList,
        GetLatestArchivedStreamByUser,
        GetArchivedStreamsInDescendingOrderByDate,
        GetArchivedStreamsInDescendingOrderByViews,

        // Payment
        GetAllPayments,
        GetPaymentsById,

        // Comment
        GetCommentsWithReceiverUsername,
        GetCommentsWithStreamId,
        GetRepliesWithStreamId,
        GetCommentWithId,
        DeleteComment,

        // View
        GetViewsWithViewer,
        GetViewsWithStreamId,
        GetViewsWithViewerAndStreamId,
        GetViewsWithChannelSince,
        GetViewsWithViewerAndChannelSince,

        //Chats
        GetAllChats,
        GetAllChatsWithIdAndVideoId,
        DeleteAllChatsWithId,

        //Follow
        GetAllFolloweesWithId,
        GetAllFollowersWithId,
        GetFollowerAndFollowee,
        GetNumberOfFollowers,
        DeleteFollower,

        //TopicFollow
        GetTopicFollowsByFollower,
        GetTopicFollowsBySubject,

        //TopicTutor
        GetTopicTutorsByTutor,
        GetTopicTutorsBySubject,

        //Schedule
        GetScheduleWithUserUsername,
        GetAllScheduledStreamsWithSearchTerm,
        GetScheduleWithId,
        DeleteScheduleTaskWithId,
        DeletePastScheduledTasks,
        UpdateTimezonesOfScheduledTasks,
        GetAllScheduledStreams,

        // Misc
        UserProfile,       //Use For Session

        //Notifications
        GetNotificationsWithReceiver,
        DeleteNotificationWithId,
        DeleteNotificationWithObjectId,
        UpdateNotificationToSeen,
        GetUnseenNotifications
    }
}
