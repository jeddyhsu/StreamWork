using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamWork.Framework;

namespace StreamWork.Models
{
	public class DonationAttempt : IStorageBase<DonationAttempt>
	{
		[Key]
		public string Id { get; set; }
		public string Student { get; set; }
		public string Tutor { get; set; }
		public DateTime TimeSent { get; set; }

		[Timestamp]
		public byte[] RowVersion { get; set; }

		public virtual void Configure(EntityTypeBuilder<DonationAttempt> builder) { }
	}
}
