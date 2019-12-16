
namespace StreamWork.HelperClasses
{
    public class Enums { }

    public enum JsonResponse
    {
        Success,
        Failed,
        Tutor,
        Student,
    }

    public enum QueryHeaders
    {
        CurrentUser,
        AllUserChannelsThatAreStreaming,
        AllUserChannelsThatAreStreamingWithSpecifiedSubject,
        UserArchivedVideosBasedOnSubject,
        AllSignedUpUsersWithPassword,
        CurrentUserChannel,
        UserArchivedVideos,
        AllArchivedVideos,
        AllApprovedTutors,
        UserProfile       //Use For Session
    }
}
