using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class UserChannel : IStorageBase<UserChannel>
    {
        //Tutors Only
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string ChannelKey { get; set; }
        public string StreamSubject { get; set; }
        public string StreamTitle { get; set; }
        public string StreamThumbnail { get; set; }
        public string ChatId { get; set; }

        public string ProfilePicture {get;set;}

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<UserChannel> builder)
        {

        }
    }
}
