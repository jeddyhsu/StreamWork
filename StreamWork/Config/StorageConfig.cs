using System.Collections.Generic;

namespace StreamWork.Config
{
    public enum StorageTypes
    {
        MongoDB,
        SqlServer
    }

    public class StorageConfig
    {
        public List<EntityModel> EntityModels { get; set; } = new List<EntityModel>();

        public StorageTypes GetStorageType<T>()
        {
            return EntityModels.Find(c => c.Name.Equals(typeof(T).Name)).StorageType;
        }
    }

    public class EntityModel
    {
        public string Name { get; set; }
        public StorageTypes StorageType { get; set; }
        public bool QualifyId { get; set; }
        public List<EntityModelQuery> Queries { get; set; } = new List<EntityModelQuery>();
    }

    public class EntityModelQuery
    {
        public string QueryId { get; set; }
        public string Query { get; set; }
    }
}
