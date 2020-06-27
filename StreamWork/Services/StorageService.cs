using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
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
        public async Task<T> Get<T> (SQLQueries query, params string[] parameters) where T : class
        {
            List<T> results = await DataStore.GetListAsync<T>(connectionString, config.Value, query.ToString(), parameters.Cast<string>().ToList());
            if (results.Count == 0)
            {
                return default;
            }
            return results[0];
        }

        public async Task<List<T>> GetList<T> (SQLQueries query, params string[] parameters) where T : class
        {
            return await DataStore.GetListAsync<T>(connectionString, config.Value, query.ToString(), parameters.Cast<string>().ToList());
        }

        public async Task<bool> Run<T>(SQLQueries query, params string[] parameters) where T : class //Tom you probably dont know about this one but I created this so that we can update and delete directly to the database without return elements, such as a mass delete or mass update
        {
            return await DataStore.RunQueryAsync<T>(connectionString, config.Value, query.ToString(), parameters.Cast<string>().ToList());
        }

        public async Task Save<T>(string id, object obj) where T : class
        {
            await DataStore.SaveAsync(connectionString, config.Value, new Dictionary<string, object> { { "Id", id} }, (T)obj);
        }
    }
}
