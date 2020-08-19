using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels.Joins
{
    public class TutorSubject : IStorageBase<TutorSubject> //i am going to find another way to do this, but doins
    {
        [Key]
        public string Username { get; set; }
        public string Name { get; set; }
        public string ProfileCaption { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfileColor{ get; set; }
        public string Topic { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<TutorSubject> builder)
        {

        }
    }
}
