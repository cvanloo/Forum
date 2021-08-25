using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Model
{
	public class SearchQuery
	{
		public List<string> TitleStrings = new();
		public List<Tag> Tags = new();
		public List<User> Users { get; set; } = new();
		public SortOrder SortBy { get; set; } = SortOrder.NewestFirst;
		public DateTime? TimeStamp { get; set; }  = null;
		public User SavedByUser { get; set; } = null;
		public List<AdvancedQuery> AdvancedQueries { get; set; } = new();

		public enum SortOrder
		{
			NewestFirst,
			OldestFirst
		}

		public delegate IEnumerable<Thread> AdvancedQuery(IEnumerable<Thread> query);

		/// <summary>
		/// Build a query from search options.
		/// </summary>
		/// <returns>IEnumerable query.</returns>
		public IEnumerable<Thread> ConstructQuery(Database dbContext)
		{
			/* ~~ LINQ ~~
			 * // 1. Data source
			 * List<Thread> threads = new List<Thread> {new(), new(), new()}; // test list

			 * // 2. Query creation, no elements are actually being retrieved
			 * // `threadQuery` never holds the actual results of the query and can be
			 * // reused to retrieve from an updated state of the database.
			 * var threadQuery =
			 * 	from thr in threads
			 * 	where thr.Title == "test"
			 * 	select thr;

			 * // 3. Query execution, elements are retrieved (deferred execution)
			 * foreach (var t in threadQuery)
			 * {
			 * 	// ...
			 * }
			 *
			 * // Queries like `Count`, `Max` and `First`, as well as `ToList` and `ToArray` force
			 * // immediate execution of the query, since they themselves use a foreach loop.
			 * // Note that they all return a single value and not an `IEnumerable` collection!
			 */

			IEnumerable<Thread> query = dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.AsSplitQuery();

			// Saved only
			if (SavedByUser is not null)
				query = query.Where(t => SavedByUser.SavedThreads.Contains(t));

			// Title
			if (TitleStrings.Any())
				query = query.Where(t => TitleStrings.Contains(t.Title));

			// Tags
			if (Tags.Any())
				query = query.Where(t => t.Tags.Intersect(Tags).Any());

			// Users
			if (Users.Any())
				query = query.Where(t => Users.Contains(t.Creator));

			// Sort order
			query = SortBy switch
			{
				SortOrder.NewestFirst => query.OrderByDescending(t => t.Created),
				SortOrder.OldestFirst => query.OrderBy(t => t.Created),
				_ => query
			};

			// Timestamp
			if (TimeStamp is not null)
			{
				query = SortBy switch
				{
					// `t.Created` happened before (earlier) `TimeStamp` (further in the past)
					SortOrder.NewestFirst => query.Where(t => t.Created < TimeStamp), // `<` is overwritten by `DateTime.op_GreaterThan`.
					// `t.Created` happened after (later) `TimeStamp` (nearer to the present)
					SortOrder.OldestFirst => query.Where(t => t.Created > TimeStamp),
					_ => query
				};
			}

			// Advanced
			foreach (var del in AdvancedQueries)
			{
				query = del(query);
			}

			return query;
		}
	}
}