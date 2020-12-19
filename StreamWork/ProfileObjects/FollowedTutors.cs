using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ProfileObjects
{
    public class FollowedTutors
    {
        public Profile TutorProfile;
        public List<Video> PreviousStreams;
        public ScheduledStream LatestScheduledStream;
        public string FollowValue;

        public FollowedTutors(Profile TutorProfile, List<Video> PreviousStreams, ScheduledStream LatestScheduledStream, string FollowValue)
        {
            this.TutorProfile = TutorProfile;
            this.PreviousStreams = PreviousStreams;
            this.LatestScheduledStream = LatestScheduledStream;
            this.FollowValue = FollowValue;
        }
    }
}
