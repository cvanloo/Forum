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
				return Threads.Count(t => t.Created.CompareTo(yesterday) > 0);
			}
		}

		public int CompareTo(object obj)
		{
			if (obj is null) return 1;

			// `as` never throws an exception, if the cast is not possible, it will just assign `null`
			// var otherTag = obj as Tag;
			// An explicit cast throws an exception if the cast is not possible.
			var otherTag = (Tag) obj;

			if (otherTag.Popularity > Popularity)
				return 1;

			if (otherTag.Popularity < Popularity)
				return -1;

			return 0;
		}

		/// <summary>
		/// Create Tags from string.
		/// </summary>
		/// <param name="strTags">String containing tags, delimited by spaces ' ' and '#'.</param>
		/// <returns>A list containing the tags.</returns>
		public static List<Tag> ParseTags(string strTags)
		{
			if (string.IsNullOrEmpty(strTags)) return null;
			
			var tags = new List<Tag>();
			var splitTags = strTags.Split(' ', '#');

			foreach (var strTag in splitTags)
			{
				tags.Add(new Tag
				{
					Name = strTag
				});
			}

			return tags;
		}
		
		/// <summary>
		/// Create tags from string and save to database using context.
		/// </summary>
		/// <param name="dbContext">The context to use.</param>
		/// <param name="strTags">String containing tags, delimited by spaces ' ' and '#'.</param>
		/// <returns>A list containing the tags.</returns>
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
