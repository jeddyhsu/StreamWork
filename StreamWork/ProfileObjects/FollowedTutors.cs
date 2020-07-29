using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.ProfileObjects
{
    public class FollowedTutors
    {
        public UserLogin Tutor;
        public List<UserArchivedStreams> PreviousStreams;
        public Schedule LatestScheduledStream;
        public string FollowValue;

        public FollowedTutors(UserLogin Tutor, List<UserArchivedStreams> PreviousStreams, Schedule LatestScheduledStream, string FollowValue)
        {
            this.Tutor = Tutor;
            this.PreviousStreams = PreviousStreams;
            this.LatestScheduledStream = LatestScheduledStream;
            this.FollowValue = FollowValue;
        }
    }
}
