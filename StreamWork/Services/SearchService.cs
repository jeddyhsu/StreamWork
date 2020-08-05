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

        public async Task<List<Channel>> SearchChannels(string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<Channel>(SQLQueries.GetAllUserChannelsThatAreStreaming);
                }
                else
                {
                    return await storage.GetList<Channel>(SQLQueries.GetUserChannelsBySearchTerm, searchQuery.ToLower());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<Channel>(SQLQueries.GetUserChannelWithSubject, subject);
                }
                else
                {
                    return await storage.GetList<Channel>(SQLQueries.GetUserChannelsBySubjectAndSearchTerm, subject, searchQuery.ToLower());
                }
            }
        }

        public async Task<List<Video>> SearchVideos(string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<Video>(SQLQueries.GetAllArchivedStreams);
                }
                else
                {
                    return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSearchTerm, searchQuery.ToLower());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSubject, subject);
                }
                else
                {
                    return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSubjectAndSearchTerm, subject, searchQuery.ToLower());
                }
            }
        }
    }
}
