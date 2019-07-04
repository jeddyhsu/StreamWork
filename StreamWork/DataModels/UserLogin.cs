using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models
{
    public class UserLogin : IStorageBase<UserLogin>
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProfileType { get; set; }
        public string ProfileCaption { get; set; }
        public string ProfileParagraph { get; set; }
        public string ProfilePicture { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<UserLogin> builder)
        {

        }
    }
}
