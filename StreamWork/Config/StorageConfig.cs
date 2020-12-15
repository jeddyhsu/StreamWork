using System.Collections.Generic;

namespace StreamWork.Config
{
    public enum StorageTypes
    {
        MongoDB,
        AzureTable
    }

    public class StorageConfig
    {
        public List<DataStorageConfig> DataStorageList { get; set; } = new List<DataStorageConfig>();
    }

    public class DataStorageConfig
    {
        public StorageTypes Type { get; set; }
        public string ConnectionString { get; set; }
        public List<EntityModel> EntityModels { get; set; }

        public DataStorageConfig()
        {
            EntityModels = new List<EntityModel>();
        }
    }

    public class EntityModel
    {
        public string Name { get; set; }
        public string QueryId { get; set; }
        public string Query { get; set; }
    }
}
