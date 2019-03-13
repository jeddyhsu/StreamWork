using Microsoft.EntityFrameworkCore;

namespace StreamWork.Framework
{
    public interface IStorageBase<T> : IEntityTypeConfiguration<T> where T : class
    {
        byte[] RowVersion { get; set; }
    }
}