using System;
using System.ComponentModel.DataAnnotations;

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

		[Required]
		public bool Used { get; set; }
	}
}
