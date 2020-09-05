using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels.Joins
{
    //the queries for this sorts tutors from MOST followers to LEAST
    public class TutorSubject : IStorageBase<TutorSubject> //i am going to find another way to do this, but doing this for beta launch
    {
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string ProfileCaption { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfileColor{ get; set; }
        public string Topic { get; set; }
        public string ProfileBanner { get; set; }
        public string College { get; set; }
        public string TopicColor { get; set; }
        public int FollowerCount { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<TutorSubject> builder)
        {

        }
    }
}
