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
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class StorageService
    {
        protected readonly string connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        protected readonly IOptionsSnapshot<StorageConfig> config;

        protected static Dictionary<Type, string> collectionNames = new Dictionary<Type, string>
        {
            { typeof(EmailVerification), "email_verifications" },
            { typeof(User), "users" },
            { typeof(Account), "accounts" },
            { typeof(Profile), "profiles" },
            { typeof(Channel), "channels" },
            { typeof(ScheduledStream), "scheduled_streams" },
            { typeof(Stream), "streams" },
            { typeof(Video), "videos" },
            { typeof(Tag), "tags" },
            { typeof(Chat), "chats" },
            { typeof(Comment), "comments" },
            { typeof(Notification), "notifications" },
            { typeof(Donation), "donations" },
            { typeof(DebugLog), "debug_logs" }
        };

        public StorageService([FromServices] IOptionsSnapshot<StorageConfig> config)
        {
            this.config = config;
        }

        public async Task<T> Get<T>(string id) where T : StorageBase //GET record with id
        {
            return await DataStore.GetAsync<T>(config.Value.DataStorageList.First(), collectionNames[typeof(T)], id);
        }

        public async Task<T> Get<T>(MongoQueries query, params string[] parameters) where T : StorageBase, new() //GET record with query and params
        {
            return (await DataStore.GetListAsync<T>(config.Value.DataStorageList.First(), parameters.ToList(), query.ToString()))[0];
        }

        public async Task<bool> Save<T>(string id, T obj) where T : StorageBase //SAVE record with id and object
        {
            return await DataStore.SaveAsync(config.Value.DataStorageList.First(), collectionNames[typeof(T)], obj, id);
        }

        public async Task<List<T>> GetList<T>(MongoQueries query, params string[] parameters) where T : StorageBase, new() //GETLIST records with query and params
        {
            return await DataStore.GetListAsync<T>(config.Value.DataStorageList.First(), parameters.ToList(), query.ToString());
        }

        public async Task<bool> Delete<T>(string id) where T : StorageBase //DELETE record with id
        {
            return await DataStore.DeleteAsync<T>(config.Value.DataStorageList.First(), collectionNames[typeof(T)], id);
        }

        public async Task<bool> DeleteMany<T>(MongoQueries query, params string[] parameters) where T : StorageBase //DELETE records with id and params
        {
            return await DataStore.DeleteManyAsync<T>(config.Value.DataStorageList.First(), collectionNames[typeof(T)], parameters.ToList(), query.ToString());
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
