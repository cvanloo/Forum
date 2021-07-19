using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Entity
{
	public class Tag
	{
		[Key, Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public ICollection<Thread> Threads { get; set; }

		[NotMapped]
		public int Popularity { get; set; } = 0;
	}
}
