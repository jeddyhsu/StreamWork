using System;
namespace StreamWork.Base
{
    public interface IStorageBase
    {
        string Id { get; set; }
        string PartitionKey { get; set; }
        string RowKey { get; }
        string Collection { get; set; }
        DateTime CreatedDT { get; set; }
        DateTime LastModifiedDT { get; set; }
        bool Deleted { get; set; }
        bool Sync { get; set; }
        DateTimeOffset Timestamp { get; set; }
    }
}
