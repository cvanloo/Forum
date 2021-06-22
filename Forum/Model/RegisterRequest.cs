using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Model
{
	public class RegisterRequest
	{
		[Required]
		public string Username { get; set; }

		public string DisplayName { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		[Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
		public string RepeatPassword { get; set; }
	}
}
