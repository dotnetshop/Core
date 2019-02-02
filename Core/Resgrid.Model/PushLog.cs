﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resgrid.Model
{
	[Table("PushLogs")]
	public class PushLog: IEntity
	{
		[Key]
		[Required]
		public int PushLogId { get; set; }

		[Required]
		public string MessageId { get; set; }

		[Required]
		public string ChannelUri { get; set; }

		[Required]
		public string Status { get; set; }

		[Required]
		public string Connection { get; set; }

		[Required]
		public string Subscription { get; set; }

		[Required]
		public string Notification { get; set; }

		[Required]
		public string Exception { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[NotMapped]
		public object Id
		{
			get { return PushLogId; }
			set { PushLogId = (int)value; }
		}
	}
}