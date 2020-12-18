using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class User : IStorageBase<User>
    {
        [Key]
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string AccountId { get; set; }
        public string ProfileId { get; set; }
        public string ChannelId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<User> builder)
        {

        }
    }
}
