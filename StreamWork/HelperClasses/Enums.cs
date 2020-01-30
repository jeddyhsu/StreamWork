
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
        AllUserChannelsThatAreStreaming,
        AllUserChannelsThatAreStreamingWithSpecifiedSubject,
        UserArchivedVideosBasedOnSubject,
        AllSignedUpUsersWithPassword,
        CurrentUserChannel,
        UserArchivedVideos,
        AllArchivedVideos,
        ArchivedVideosByStreamId,
        AllApprovedTutors,
        UserProfile       //Use For Session
    }
}
