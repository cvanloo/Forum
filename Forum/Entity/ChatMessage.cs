using System;
using System.ComponentModel.DataAnnotations;

namespace Forum.Entity
{
	public class ChatMessage
	{
		[Key,Required]
		public int Id { get; set; }

		[Required]
		public User Sender { get; set; }

		public ChatMessage Parent { get; set; }
		
		[Required]
		public string Content { get; set; }

		[Required]
		public DateTime Sent { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
		
		[Required]
		public Chat Chat { get; set; }
	}
}
