﻿using System.ComponentModel.DataAnnotations;
using Resgrid.Model;

namespace Resgrid.Web.Areas.User.Models.Notes
{
	public class NewNoteView
	{
		public string Message { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Body { get; set; }
		public string IsAdminOnly { get; set; }
		public string Category { get; set; }
	}
}