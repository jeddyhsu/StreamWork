using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Base;
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

        public async Task<T> Get<T>(string id) where T : StorageBase
        {
            var result = await DataStore.GetAsync<T>(config.Value.DataStorageList.First(), typeof(T).Name.ToLower(), id);
            return result;
        }

        public async Task<bool> Save<T>(string id, T obj, string t) where T : StorageBase
        {
            return await DataStore.SaveAsync(config.Value.DataStorageList.First(), typeof(T).Name.ToLower(), obj, id);
        }

        public async Task<List<T>> GetList<T>(string query, params string[] parameters) where T : StorageBase, new()
        {
            return await DataStore.GetListAsync<T>(config.Value.DataStorageList.First(), null, query);
        }

        public async Task<T> Get<T>(SQLQueries query, params string[] parameters) where T : class
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

        public async Task<T> CallJSON<T>(string url, string authToken) where T : class
        {
            return (T)await DataStore.CallAPIJSON<T>(url, authToken);
        }

        public async Task<T> CallJSON<T>(string url, StringContent content) where T : class
        {
            return (T)await DataStore.CallAPIJSON<T>(url, content);
        }

        public async Task<T> CallJSON<T>(string url) where T : class
        {
            return (T)await DataStore.CallAPIJSON<T>(url);
        }

        public async Task<T> CallXML<T>(string url) where T : class
        {
            return (T)await DataStore.CallAPIXML<T>(url);
        }
    }
}
