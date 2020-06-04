using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Follow : IStorageBase<Follow>
    {
        [Key]
        public string Id { get; set; }
        public string FollowerId { get; set; }
        public string FollowerUsername { get; set; }
        public string FollowerEmail { get; set; }
        public string FolloweeId { get; set; }
        public string FolloweeUsername { get; set; }
        public string FolloweeEmail { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Follow> builder)
        {

        }
    }
}
