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
			if (obj is null) return 1;

			var otherTag = obj as Tag;

			// ReSharper disable once PossibleNullReferenceException
			if (otherTag.Popularity > Popularity)
				return 1;

			if (otherTag.Popularity < Popularity)
				return -1;

			return 0;
		}
		
		// TODO: Don't take dbcontext, just parse and return without adding them to the db(context).
		public static List<Tag> ParseTags(Model.Database dbContext, string strTags)
        {
        	if (string.IsNullOrEmpty(strTags)) return null;
       
       		var tags = new List<Tag>();
       		var separatedTags = strTags.Split(' ', '#');
       
       		// ReSharper disable once LoopCanBeConvertedToQuery
       		foreach (var tagStr in separatedTags)
       		{
       			if (string.IsNullOrEmpty(tagStr)) continue;
        
       			// Get tag from db if it exists, create a new tag if it doesn't.
       			var tag = dbContext.Tags.FirstOrDefault(t => t.Name == tagStr);
        
       			if (tag == null)
       			{
       				var newTag = new Tag { Name = tagStr };
       				dbContext.Tags.Add(newTag);
       				dbContext.SaveChanges();
       				tags.Add(newTag);
       			}
       			else if (!tags.Contains(tag))
       			{
       				tags.Add(tag);
       			}
       		}
       
       		return tags;
        }
	}
}
