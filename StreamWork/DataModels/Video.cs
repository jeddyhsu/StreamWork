﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Video : IStorageBase<Video>
    {
        [Key]
        public string Id { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public float ViewCount { get; set; }
        public string Topic { get; set; }
        public string[] TagIds { get; set; }
        public string[] ChatIds { get; set; }
        public string[] CommentIds { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual void Configure(EntityTypeBuilder<Video> builder)
        {

        }
    }
}
