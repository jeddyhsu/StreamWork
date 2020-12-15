using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ProfileObjects
{
    public class FollowedTutors
    {
        public Profiles Tutor;
        public List<Video> PreviousStreams;
        public Schedule LatestScheduledStream;
        public string FollowValue;

        public FollowedTutors(Profiles Tutor, List<Video> PreviousStreams, Schedule LatestScheduledStream, string FollowValue)
        {
            this.Tutor = Tutor;
            this.PreviousStreams = PreviousStreams;
            this.LatestScheduledStream = LatestScheduledStream;
            this.FollowValue = FollowValue;
        }
    }
}
