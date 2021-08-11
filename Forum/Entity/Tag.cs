using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
				var yesterday = DateTime.Now;
				yesterday = yesterday.AddHours(-24);
				// ReSharper disable once InconsistentNaming
				var last24hThreads = Threads.Where(t => t.Created.CompareTo(yesterday) > 0).ToList();
				return last24hThreads.Count;
			}
		}

		public int CompareTo(object obj)
		{
			if (null == obj) return 1;

			var otherTag = obj as Tag;

			// ReSharper disable once PossibleNullReferenceException
			if (otherTag.Popularity > Popularity)
				return 1;

			if (otherTag.Popularity < Popularity)
				return -1;

			return 0;
		}
	}
}
