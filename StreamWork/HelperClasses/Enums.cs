
namespace StreamWork.HelperClasses
{
    public class Enums{}

    public enum JsonResponse
    {
        Success,
        Failed
    }

    public enum QueryHeaders
    {
        CurrentUser,
        AllUserChannelsThatAreStreaming,
        AllUserChannelsThatAreStreamingWithSpecifiedSubject,
        UserArchivedVideosBasedOnSubject,
        CurrentUserChannel,
        UserArchivedVideos,
        UserProfile       //Use For Session
    }
}
