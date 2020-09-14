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

        public async Task<bool> Run<T>(SQLQueries query, params string[] parameters) where T : class
        {
            return await DataStore.RunQueryAsync<T>(connectionString, config.Value, query.ToString(), parameters.Cast<string>().ToList());
        }

        public async Task<bool> Delete<T>(string id) where T : class
        {
            return await DataStore.DeleteAsync<T>(connectionString, config.Value, new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<bool> Save<T>(string id, T obj) where T : class
        {
            return await DataStore.SaveAsync(connectionString, config.Value, new Dictionary<string, object> { { "Id", id } }, obj);
        }

        public T CallJSON<T>(string url, string authToken) where T : class
        {
            return DataStore.CallAPIJSON<T>(url, authToken);
        }

        public T CallJSON<T>(string url) where T : class
        {
            return DataStore.CallAPIJSON<T>(url);
        }

        public T CallXML<T>(string url) where T : class
        {
            return (T)DataStore.CallAPIXML<T>(url);
        }
    }
}
