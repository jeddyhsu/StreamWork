using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StreamWork.Config;

namespace StreamWork.Base
{
    public class SQLServerClient : DbContext
    {
        private string _connectionString;
        private readonly StorageConfig _storageConfig;

        public SQLServerClient() { }

        public SQLServerClient(string connectionString, StorageConfig storageConfig)
        {
            _connectionString = connectionString;
            _storageConfig = storageConfig;
        }

        public async Task<List<T>> GetDataAsync<T>(string connectionString,
                                                   string query,
                                                   List<object> selector = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(connectionString) == false)
                _connectionString = connectionString;

            if (string.IsNullOrWhiteSpace(_connectionString) == true ||
                string.IsNullOrWhiteSpace(query) == true)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(query);

            if (selector?.Count > 0)
            {
                if (query.Contains("@p") == false)
                {
                    bool firstProperty = true;
                    var sourceType = selector.GetType();
                    for (int i = 0; i < selector.Count; i++)
                    {
                        if (firstProperty == false)
                            sb.Append(", ");
                        else
                        {
                            sb.Append(" ");
                            firstProperty = false;
                        }

                        sb.Append("{");
                        sb.Append(i);
                        sb.Append("}");
                    }
                }

                return await Set<T>().FromSql(sb.ToString(), selector.ToArray()).AsNoTracking().ToListAsync<T>();
            }
            else
                return await Set<T>().FromSql<T>(sb.ToString()).AsNoTracking().ToListAsync<T>();
        }

        public async Task<bool> DeleteDataAsync(string query)
        {
            using (var context = Database.GetDbConnection())
            {
                await context.OpenAsync();
                using (var command = context.CreateCommand())
                {
                    command.CommandText = query;
                    var result = await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assembly = Assembly.GetExecutingAssembly();
            // get ApplyConfiguration method with reflection
            var applyEntityTypeConfigMethods = typeof(ModelBuilder).GetMethods().First(c => c.Name.Equals("ApplyConfiguration"));
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(c => c.IsClass && !c.IsAbstract))
            {
                if (_storageConfig.EntityModels.Exists(c => c.Name.Equals(type.Name) && c.StorageType == StorageTypes.SqlServer))
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        // if type implements interface IEntityTypeConfiguration<SomeEntity>
                        if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                        {
                            // make concrete ApplyConfiguration<SomeEntity> method
                            var applyConcreteMethod = applyEntityTypeConfigMethods.MakeGenericMethod(interfaceType.GenericTypeArguments[0]);
                            // and invoke that with fresh instance of your configuration type
                            applyConcreteMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                            break;
                        }
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
