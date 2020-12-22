using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class Notification : StorageBase
    {
        [BsonElement("active")]
        public bool Active { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("data")]
        public object Data { get; set; }
    }
}
