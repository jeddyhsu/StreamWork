using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StreamWork.Base;
using StreamWork.Config;

namespace StreamWork.Core
{
    public static class DataStore
    {
        private const string databaseName = "StreamWork";
        private const string connectionString = "mongodb+srv://rarun:Rithvik2000@streamworkdb.4dahn.mongodb.net/" + databaseName + "?retryWrites=true&w=majority";

        private static IMongoCollection<T> GetMongoCollection<T>(string collection)
        {
            return new MongoClient(connectionString).GetDatabase(databaseName).GetCollection<T>(collection);
        }

        /// <summary>
        /// Get data from storage
        /// All exceptions are bubbled up.
        /// </summary>
        public async static Task<T> GetAsync<T>(DataStorageConfig storageConfig, string collection, string id) where T : StorageBase
        {
            if (storageConfig.Type == StorageTypes.MongoDB)
            {
                var mongoCollection = GetMongoCollection<T>("debug");
                if (mongoCollection == null)
                    return default(T);

                var mongoDoc = mongoCollection.Find(new FilterDefinitionBuilder<T>().Eq("Id", id));
                return await mongoDoc?.AnyAsync() == true ? await mongoDoc.FirstAsync<T>() : default(T);
            }

            return default(T);
        }

        /// <summary>
        /// Saves data to storage.
        /// All exceptions are bubbled up.
        /// </summary>
        public async static Task<bool> SaveAsync<T>(DataStorageConfig storageConfig,
                                                    string collectionName,
                                                    T dataObject,
                                                    string id) where T : StorageBase
        {
            if (string.IsNullOrWhiteSpace(dataObject.Collection) == true)
                dataObject.Collection = typeof(T).Name;

            if (storageConfig.Type == StorageTypes.MongoDB)
            {
                var mongoCollection = GetMongoCollection<T>(collectionName);
                if (mongoCollection == null)
                    await new MongoClient(storageConfig.ConnectionString).GetDatabase(databaseName).CreateCollectionAsync(collectionName);

                await mongoCollection.ReplaceOneAsync(new FilterDefinitionBuilder<T>().Eq("Id", id), dataObject, new ReplaceOptions { IsUpsert = true });

            }

            return true;
        }

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

            var query = FormatQuery<T>(_connectionString, storageConfig, queryId, parameters);

            using (var sqlServerClient = new SQLServerClient(_connectionString, storageConfig))
            {
                return await sqlServerClient.GetDataAsync<T>(_connectionString, query, selector);
            }
        }


        public async static Task<bool> RunQueryAsync<T>(string _connectionString,
                                                          StorageConfig storageConfig,
                                                          string queryId,
                                                          List<string> parameters = null,
                                                          bool deleteAll = false,
                                                          List<object> selector = null,
                                                          string udfQuery = "") where T : class
        {

            var query = FormatQuery<T>(_connectionString, storageConfig, queryId, parameters);

            using (var sqlServerClient = new SQLServerClient(_connectionString, storageConfig))
            {
                 if(await sqlServerClient.RunQueryAsync(query)) return true;
                 return false;
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

        private static string FormatQuery<T>(string _connectionString,
                                             StorageConfig storageConfig,
                                             string queryId,
                                             List<string> parameters = null,
                                             bool deleteAll = false,
                                             List<object> selector = null,
                                             string udfQuery = "")
                                                         
        {
            var query = string.Empty;
            var name = typeof(T).Name;
            //if (storageConfig?.EntityModels?.Exists(c => c.Name.Equals(name)) == true)
            //{
            //    if (string.IsNullOrWhiteSpace(queryId) == true)
            //        query = storageConfig.EntityModels.First(c => c.Name.Equals(name)).Queries.First().Query;
            //    else
            //    {
            //        query = storageConfig.EntityModels.First(c => c.Name.Equals(name)).Queries.Find(d => d.QueryId.Equals(queryId)).Query;
            //        if (query.Equals("*UDF") == true)
            //            query = udfQuery;
            //    }
            //}

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

            return query;
        }

        private static void Copy(object source, object destination)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            foreach (PropertyInfo sourceProperty in sourceType.GetRuntimeProperties())
            {
                if (sourceProperty.Name?.Equals("RowVersion") == false)
                {
                    PropertyInfo destinationProperty = destinationType.GetRuntimeProperty(sourceProperty.Name);
                    if (destinationProperty != null)
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                    }
                }
            }
        }

        private static string FormatKey<T>(StorageConfig storageConfig, object key)
        {
            var name = typeof(T).Name;
            //if (storageConfig.EntityModels.Find(c => c.Name.Equals(name)).QualifyId)
            //    return name + key;
            //else
            //    return key.ToString();

            return null;
        }

        //Takes Generic Type - use for any api that has json format
        public static async Task<object> CallAPIXML<T>(string URL)
        {
            try
            {
                Stream res;

                using(HttpClient client = new HttpClient())
                {
                    res = await client.GetStreamAsync(URL);
                }

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                var serialized = serializer.Deserialize(res);

                return serialized;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.InnerException);
            }

            return null;
        }


        //Takes Generic Type - use for any api that has json format with authentication
        public static async Task<object> CallAPIJSON<T>(string URL, StringContent content)
        {
            try
            {
                HttpResponseMessage res;

                using (HttpClient client = new HttpClient())
                {
                    res = await client.PostAsync(URL, content);
                }

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());

                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }

            return null;
        }

        //Takes Generic Type - use for any api that has json format with authentication
        public static async Task<object> CallAPIJSON<T>(string URL, string authenticationToken)
        {
            try
            {
                string res;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationToken);
                    res = await client.GetStringAsync(URL);
                }

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(res);

                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }

            return null;
        }

        //Takes Generic Type - use for any api that has json format with authentication
        public static async Task<object> CallAPIJSON<T>(string URL)
        {
            try
            {
                string res;

                using (HttpClient client = new HttpClient())
                {
                    res = await client.GetStringAsync(URL);
                }

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(res);

                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }

            return null;
        }
    }
}