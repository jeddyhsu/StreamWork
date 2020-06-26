using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.TutorObjects;

namespace StreamWork.Services
{
    public class StorageService
    {
        private readonly string connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly IOptionsSnapshot<StorageConfig> config;

        public StorageService([FromServices] IOptionsSnapshot<StorageConfig> config)
        {
            this.config = config;
        }

        // I'm still debating whether to make method public.
        // It would make this class much shorter,
        // But, on the other hand, I don't want to have to deal with the queries in the rest of the code, if I can avoid it
        private async Task<T> Get<T> (SQLQueries query, params string[] parameters) where T : class
        {
            List<T> results = await DataStore.GetListAsync<T>(connectionString, config.Value, query.ToString(), parameters.Cast<string>().ToList());
            if (results.Count == 0)
            {
                return default;
            }
            return results[0];
        }

        private async Task<List<T>> GetList<T> (SQLQueries query, params string[] parameters) where T : class
        {
            return await DataStore.GetListAsync<T>(connectionString, config.Value, query.ToString(), parameters.Cast<string>().ToList());
        }

        public async Task<UserLogin> GetUser(string name)
        {
            return await Get<UserLogin>(SQLQueries.GetUserWithUsername, name);
        }

        public async Task<UserChannel> GetChannel(string name)
        {
            return await Get<UserChannel>(SQLQueries.GetUserChannelWithUsername, name);
        }

        public async Task<List<UserArchivedStreams>> GetArchivedStreamsByTutor(string tutorName)
        {
            return await GetList<UserArchivedStreams>(SQLQueries.GetArchivedStreamsWithUsername, tutorName);
        }

        // TODO
        public List<Section> GetSectionsByTutor(string tutorName)
        {
            return new List<Section>();
        }

        // TODO
        public List<Topic> GetTopicsByTutor(string tutorName)
        {
            return new List<Topic>();
        }

        // TODO
        public List<Recommendation> GetRecommendationsToTutor(string tutorName)
        {
            return new List<Recommendation>();
        }

        // TODO
        public List<Schedule> GetSchedule(string tutorName)
        {
            return new List<Schedule>();
        }

        // TODO
        public int GetFollowerCountOf(string tutorName)
        {
            return 0;
        }
    }
}
