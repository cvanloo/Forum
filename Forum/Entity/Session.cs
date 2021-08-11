using System.ComponentModel.DataAnnotations;

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
