using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Entity
{
	public class PwReset
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }
		[Required]
		public User User { get; set; }

		[Required]
		public DateTime Timestamp { get; set; }

		[Required]
		public string Token { get; set; }
	}
}
