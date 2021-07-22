using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Entity
{
	public class Tag : IComparable
	{
		[Key, Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public ICollection<Thread> Threads { get; set; }

		[NotMapped]
		public int Popularity
		{
			get
			{
				DateTime yesterday = DateTime.Now;
				yesterday = yesterday.AddHours(-24);
				var last24hThreads = Threads.Where(t => t.Created.CompareTo(yesterday) > 0).ToList();
				return last24hThreads.Count;
			}
		}

		public int CompareTo(object obj)
		{
			if (null == obj) return 1;

			Tag otherTag = obj as Tag;

			if (otherTag.Popularity > Popularity)
				return 1;

			if (otherTag.Popularity < Popularity)
				return -1;

			return 0;
		}
	}
}
