using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ProfileObjects
{
    public class FollowedTutors
    {
        public Profile Tutor;
        public List<Video> PreviousStreams;
        public Schedule LatestScheduledStream;
        public string FollowValue;

        public FollowedTutors(Profile Tutor, List<Video> PreviousStreams, Schedule LatestScheduledStream, string FollowValue)
        {
            this.Tutor = Tutor;
            this.PreviousStreams = PreviousStreams;
            this.LatestScheduledStream = LatestScheduledStream;
            this.FollowValue = FollowValue;
        }
    }
}
