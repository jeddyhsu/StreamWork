
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
        CurrentUser,
        CurrentUserChannelFromId,
        AllUserChannelsOfAnyUsernameThatAreStreaming,
        AllUserChannelsThatAreStreaming,
        AllUserChannelsThatAreStreamingWithSpecifiedSubject,
        UserArchivedVideosBasedOnSubject,
        AllSignedUpUsersWithPassword,
        CurrentUserChannel,
        UserArchivedVideos,
        AllArchivedVideos,
        ArchivedVideosByStreamId,
        AllApprovedTutors,
        UserProfile,       //Use For Session
        RecommendationsByTutor,
        UserChannelsBySearchTerm,
        UserChannelsBySubjectAndSearchTerm,
        ArchivedVideosBySearchTerm,
        ArchivedVideosBySubjectAndSearchTerm,
        AllStudents,
        AllSignedUpUsers
    }
}
