using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.WindowsAzure.Storage.Table;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace StreamWork.Base
{
    public class StorageBase : IStorageBase, ITableEntity
    {
        [Key]
        [IgnoreProperty]
        [System.Text.Json.Serialization.JsonPropertyName("_id")]
        public string Id { get; set; }

        [IgnoreProperty]
        [System.Text.Json.Serialization.JsonPropertyName("user_id")]
        public string UserId { get; set; }

        //[Ignore]
        [IgnoreProperty]
        [BsonIgnore]
        public string QueryId { get; set; }

        //[Ignore]
        [IgnoreProperty]
        [BsonIgnore]
        public string QueryParameter { get; set; }

        //[Ignore]
        [JsonIgnore]
        [BsonIgnore]
        public string PartitionKey { get { return UserId; } set { UserId = value; } }

        //[Ignore]
        [JsonIgnore]
        [BsonIgnore]
        public string RowKey { get { return Id; } set { Id = value; } }

        //[Ignore]
        [IgnoreProperty]
        public string Collection { get; set; }

        [IgnoreProperty]
        public DateTime CreatedDT { get; set; }

        [IgnoreProperty]
        public DateTime LastModifiedDT { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        [IgnoreProperty]
        public bool Deleted { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        [IgnoreProperty]
        public bool Sync { get; set; }

        //[Ignore]
        [JsonIgnore]
        [BsonIgnore]
        public string ETag { get; set; }

        //[Ignore]
        [JsonIgnore]
        [BsonIgnore]
        public DateTimeOffset Timestamp { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            TableEntityHelper.ReadEntity(this, properties, operationContext);
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return TableEntityHelper.WriteEntity(this, operationContext);
        }

        IDictionary<string, EntityProperty> ITableEntity.WriteEntity(OperationContext operationContext)
        {
            throw new NotImplementedException();
        }
    }
}
