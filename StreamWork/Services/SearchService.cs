using System.Collections.Generic;
using System.Threading.Tasks;
using StreamWork.DataModels;
using StreamWork.DataModels.Joins;
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

        public async Task<List<Video>> SearchVideos(string subject, string searchQuery, string username = null) 
        {
            if (string.IsNullOrEmpty(subject))
            {
                if(string.IsNullOrWhiteSpace(searchQuery))
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return await storage.GetList<Video>(SQLQueries.GetAllArchivedStreams); ;
                    }
                    else
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, username);
                    }
                }
                else 
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSearchTerm, searchQuery.ToLower());
                    }
                    else
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSearchTermAndUsername, searchQuery.ToLower(), username);
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSubject, subject);
                    }
                    else
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSubjectAndUsername, subject, username);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSubjectAndSearchTerm, subject, searchQuery.ToLower());
                    }
                    else
                    {
                        return await storage.GetList<Video>(SQLQueries.GetArchivedStreamsWithSubjectAndSearchTermAndUsername, subject, searchQuery.ToLower(), username);
                    }
                }
            }
        }

        public async Task<List<Schedule>> SearchSchedule(string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<Schedule>(SQLQueries.GetAllScheduledStreams);
                }
                else
                {
                    return await storage.GetList<Schedule>(SQLQueries.GetAllScheduledStreamsWithSearchTerm, searchQuery.ToLower());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<Schedule>(SQLQueries.GetAllScheduledStreamsWithSubject, subject);
                }
                else
                {
                    return await storage.GetList<Schedule>(SQLQueries.GetAllScheduledStreamsWithSearchTermAndSubject, searchQuery.ToLower(), subject);
                }
            }
        }

        public async Task<List<TutorSubject>> SearchTutors(string subject, string searchQuery)
        {
            if (string.IsNullOrEmpty(subject))
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<TutorSubject>(SQLQueries.GetApprovedTutorSubjects);
                }
                else
                {
                    return await storage.GetList<TutorSubject>(SQLQueries.GetApprovedTutorSubjectsWithSearchTerm, searchQuery.ToLower());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    return await storage.GetList<TutorSubject>(SQLQueries.GetApprovedTutorSubjectsWithSubject, subject);
                }
                else
                {
                    return await storage.GetList<TutorSubject>(SQLQueries.GetApprovedTutorSubjectsWithSubjectAndSearchTerm, subject, searchQuery.ToLower());
                }
            }
        }
    }
}
