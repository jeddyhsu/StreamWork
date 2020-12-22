using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization.Attributes;
using StreamWork.Base;
using StreamWork.Framework;

namespace StreamWork.DataModels
{
    public class EmailVerification : StorageBase
    {
        [BsonElement("email_address")]
        public string EmailAddress { get; set; }

        [BsonElement("verification_code")]
        public string VerificationCode { get; set; }
    }
}
