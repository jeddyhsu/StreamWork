using System.Collections.Generic;
using System.Threading.Tasks;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class SearchService
    {
        private readonly StorageService storage;

        public SearchService(StorageService storage)
        {
            this.storage = storage;
        }

        public async Task<List<UserChannel>> SearchChannels(string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<UserChannel>(SQLQueries.GetAllUserChannelsThatAreStreaming);
                }
                else
                {
                    return await storage.GetList<UserChannel>(SQLQueries.GetUserChannelsBySearchTerm, searchQuery.ToLower());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<UserChannel>(SQLQueries.GetUserChannelWithSubject, subject);
                }
                else
                {
                    return await storage.GetList<UserChannel>(SQLQueries.GetUserChannelsBySubjectAndSearchTerm, subject, searchQuery.ToLower());
                }
            }
        }

        public async Task<List<UserArchivedStreams>> SearchArchivedStreams(string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<UserArchivedStreams>(SQLQueries.GetAllArchivedStreams);
                }
                else
                {
                    return await storage.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithSearchTerm, searchQuery.ToLower());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithSubject, subject);
                }
                else
                {
                    return await storage.GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithSubjectAndSearchTerm, subject, searchQuery.ToLower());
                }
            }
        }
    }
}
