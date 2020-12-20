using System;
using System.Collections.Generic;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class TopicService
    {
        private readonly StorageService storage;

        public TopicService(StorageService storage)
        {
            this.storage = storage;
        }

        public void FollowTopics(string username, List<string> topics)
        {
            //topics.ForEach(async x => {
            //    string guid = Guid.NewGuid().ToString();
            //    await storage.Save(guid, new TopicFollow
            //    {
            //        Id = guid,
            //        Follower = username,
            //        Topic = x,
            //        Since = DateTime.UtcNow,
            //    });
            //});
        }

        public void TutorTopics(string username, List<string> topics)
        {
            //topics.ForEach(async x => {
            //    string guid = Guid.NewGuid().ToString();
            //    await storage.Save(guid, new TopicTutor
            //    {
            //        Id = guid,
            //        Tutor = username,
            //        Topic = x,
            //        Since = DateTime.UtcNow,
            //        TopicColor = MiscHelperMethods.GetCorrespondingStreamColor(x)
            //    });
            //});
        }
    }
}
