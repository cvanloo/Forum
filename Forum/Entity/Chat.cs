using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum.Entity
{
	public class Chat
	{
		[Key,Required]
		public int Id { get; set; }

		[Required]
		public DateTime Created { get; set; }

		[Required]
		public bool IsDeleted { get; set; }

		public ICollection<User> Participants { get; set; }

		public ICollection<ChatMessage> Messages { get; set; }
	}
}
