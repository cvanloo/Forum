using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Entity
{
	public class Session
	{
		[Key, Required]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }
		[Required]
		public User User { get; set; }

		[Required]
		public string Identifier { get; set; }

		[Required]
		public string Value { get; set; }
	}
}
