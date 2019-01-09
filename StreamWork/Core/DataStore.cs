using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StreamWork.Base;
using StreamWork.Config;

namespace StreamWork.Core
{
    public static class DataStore
    {
        private static readonly string _databaseName = "Streamwork";
        private static readonly string _collectionName = "Streamwork";


        /// <summary>
        /// Get data from storage
        /// All exceptions are bubbled up.
        /// </summary>
        public async static Task<T> GetAsync<T>(string _connectionString,
                                                StorageConfig storageConfig, 
                                                Dictionary<string, object> keys) where T : class
        {          
            {
                using (var sqlServerClient = new SQLServerClient(_connectionString, storageConfig))
                {
                    return await sqlServerClient.FindAsync<T>(GetKeys<T>(keys, sqlServerClient));
                }
            }
        }

        /// <summary>
        /// Saves data to storage.
        /// All exceptions are bubbled up.
        /// </summary>
        public async static Task<bool> SaveAsync<T>(string _connectionString,
                                                    StorageConfig storageConfig, 
                                                    Dictionary<string, object> keys,
                                                    T dataObject) where T : class
        {
            {
                using (var sqlServerClient = new SQLServerClient(_connectionString, storageConfig))
                {
                    var readData = await GetAsync<T>(_connectionString, storageConfig, keys);
                    if (readData != null)
                    {
                        Copy(dataObject, readData);
                        sqlServerClient.Update<T>(readData);
                    }
                    else
                    {
                        sqlServerClient.Add<T>(dataObject);
                    }

                    await sqlServerClient.SaveChangesAsync();
                }
            }

            return true;
        }

        /// <summary>
        /// Delete data from storage 
        /// </summary>
        public static async Task<bool> DeleteAsync<T>(string _connectionString, 
                                                      StorageConfig storageConfig, 
                                                      Dictionary<string, object> keys) where T : class
        {
            {
                using (var sqlServerClient = new SQLServerClient(_connectionString, storageConfig))
                {
                    var readData = await GetAsync<T>(_connectionString, storageConfig, keys);
                    if (readData != null)
                    {
                        sqlServerClient.Remove<T>(readData);
                        await sqlServerClient.SaveChangesAsync();
                    }
                }
            }

            return true;
        }

        public async static Task<List<T>> GetListAsync<T>(string _connectionString,
                                                          StorageConfig storageConfig, 
                                                          string queryId,
                                                          List<string> parameters = null,
                                                          bool deleteAll = false,
                                                          List<object> selector = null,
                                                          string udfQuery = "") where T : class
        {
            var query = string.Empty;
            var name = typeof(T).Name;
            if (storageConfig?.EntityModels?.Exists(c => c.Name.Equals(name)) == true)
            {
                if (string.IsNullOrWhiteSpace(queryId) == true)
                    query = storageConfig.EntityModels.First(c => c.Name.Equals(name)).Queries.First().Query;
                else
                {
                    query = storageConfig.EntityModels.First(c => c.Name.Equals(name)).Queries.Find(d => d.QueryId.Equals(queryId)).Query;
                    if (query.Equals("*UDF") == true)
                        query = udfQuery;
                }
            }

            if (query.Contains("$Collection") == true)
                query = query.Replace("$Collection", name ?? string.Empty);

            if (query.Contains("$schema"))
                query = query.Replace("$schema", _connectionString);

            if (parameters?.Count > 0 && query.Contains("@x"))
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    var pattern = @"(?<![\w])@x" + i + @"(?![\w])";
                    query = Regex.Replace(query, pattern, parameters[i]);
                }
            }

            using (var sqlServerClient = new SQLServerClient(_connectionString, storageConfig))
            {
                return await sqlServerClient.GetDataAsync<T>(_connectionString, query, selector);
            }

        }

        private static object[] GetKeys<T>(Dictionary<string, object> keyData, DbContext context)
        {
            var keyNames = GetKeyNames<T>(context);
            Type type = typeof(T);

            object[] keys = new object[keyNames.Count];
            for (int i = 0; i < keyNames.Count; i++)
            {
                keys[i] = keyData[keyNames[i]];
            }

            return keys;
        }

        private static List<string> GetKeyNames<T>(DbContext context)
        {
            Type t = typeof(T);

            //retreive the base type
            while (t.BaseType != typeof(object))
            {
                t = t.BaseType;
            }

            return context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name).ToList();
        }

        private static void Copy(object source, object destination)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            foreach (PropertyInfo sourceProperty in sourceType.GetRuntimeProperties())
            {
                PropertyInfo destinationProperty = destinationType.GetRuntimeProperty(sourceProperty.Name);
                if (destinationProperty != null)
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                }
            }
        }

        private static string FormatKey<T>(StorageConfig storageConfig, object key)
        {
            var name = typeof(T).Name;
            if (storageConfig.EntityModels.Find(c => c.Name.Equals(name)).QualifyId)
                return name + key;
            else
                return key.ToString();
        }
    }
}