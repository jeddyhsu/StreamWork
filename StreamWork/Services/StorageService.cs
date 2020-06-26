using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class StorageService
    {
        protected readonly string connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        protected readonly IOptionsSnapshot<StorageConfig> config;

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

        public async Task<UserLogin> GetUser(SQLQueries query, string[] parameters)
        {
            return await Get<UserLogin>(query, parameters);
        }

        public async Task<List<UserLogin>> GetUsers(SQLQueries query, string[] parameters)
        {
            return await GetList<UserLogin>(query, parameters);
        }

        public async Task<UserChannel> GetChannel(SQLQueries query, string[] parameters)
        {
            return await Get<UserChannel>(query, parameters);
        }

        public async Task<UserChannel> GetChannels(SQLQueries query, string[] parameters)
        {
            return await Get<UserChannel>(query, parameters);
        }

        public async Task<UserArchivedStreams> GetArchivedStream(SQLQueries query, string[] parameters)
        {
            return await Get<UserArchivedStreams>(query, parameters);
        }

        public async Task<List<UserArchivedStreams>> GetArchivedStreams(SQLQueries query, string[] parameters)
        {
            return await GetList<UserArchivedStreams>(query, parameters);
        }

        public async Task<List<Chats>> GetChats(SQLQueries query, string[] parameters)
        {
            return await GetList<Chats>(query, parameters);
        }
    }
}
