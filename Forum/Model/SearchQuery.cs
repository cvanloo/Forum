using System;
using System.Collections.Generic;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Model
{
	// TODO: Base class
	public class SearchQuery
	{
		public List<string> TitleStrings { get; set; } = new();
		public List<Tag> Tags { get; set; } = new();
		public List<User> Users { get; set; } = new();
		public SortOrder SortBy { get; set; } = SortOrder.NewestFirst;
		public DateTime? TimeStamp { get; set; }  = null;
		public User SavedByUser { get; set; } = null;

		public enum SortOrder
		{
			NewestFirst,
			OldestFirst
		}

		/// <summary>
		/// Build a query from search options.
		/// </summary>
		/// <returns>IEnumerable query.</returns>
		public IQueryable<Thread> Construct(Database dbContext)
		{
			/* ~~ LINQ ~~
			 * // 1. Data source
			 * List<Thread> threads = new List<Thread> {new(), new(), new()}; // test list

			 * // 2. Query creation, no elements are actually being retrieved
			 * // `threadQuery` never holds the actual results of the query and can be
			 * // reused to retrieve from an updated state of the database.
			 * var threadQuery =
			 *     from thr in threads
			 *     here thr.Title == "test"
			 *     elect thr;

			 * // 3. Query execution, elements are retrieved (deferred execution)
			 * foreach (var t in threadQuery)
			 * {
			 * 	   // ...
			 * }
			 *
			 * // Queries like `Count`, `Max` and `First`, as well as `ToList` and `ToArray` force
			 * // immediate execution of the query, since they themselves use a foreach loop.
			 * // Note that they all return a single value and not an `IEnumerable` collection!
			 */

			// NOTE: `IEnumerable` (LINQ-to-object) and `IQueryable` (LINQ-to-SQL) both use deferred execution.
			// The difference is, that when further refining a query, with `IQueryable` the query will be (if possible)
			// executed in the database. With `IEnumerable`, only the original query is run in the database, for
			// additional queries all objects matching the original query need to be loaded into memory.
			// For reference see: https://stackoverflow.com/a/2876655
			IQueryable<Thread> query = dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.AsSplitQuery();

			// -- Saved only --
			if (SavedByUser is not null)
				query = query.Where(t => SavedByUser.SavedThreads.Contains(t));

			// -- Title --
			if (TitleStrings.Any())
			{
				var predicate = PredicateBuilderLinq.False<Thread>(); // if (false)
				foreach (var title in TitleStrings)
				{
					// Build up (combine) the predicates like:
					// if (false || t.Title == TitleStrings[0] || t.Title == TitleStrings[1] || ...)
					predicate = predicate.Or(t => t.Title.ToLower().Contains(title.ToLower()));
				}
				
				// Get elements Where(t => false || t.Title == TitleStrings[0] || t.Title == TitleStrings[1] || ...)
				query = query.Where(predicate);
			}

			// -- Tags --
			if (Tags.Any())
			{
				// With `PredicateBuilder` instead of `PredicateBuilderLinq`, all objects (entire tables) would
				// get loaded into memory, instead queried within the database.
				var predicate = PredicateBuilderLinq.False<Thread>();
				foreach (var tag in Tags)
				{
					predicate = predicate.Or(t => t.Tags.Contains(tag));
				}

				query = query.Where(predicate);
			}

			// -- Users --
			if (Users.Any())
			{
				var predicate = PredicateBuilderLinq.False<Thread>();
				foreach (var user in Users)
				{
					predicate = predicate.Or(t => t.Creator == user);
				}

				query = query.Where(predicate);
			}

			// -- Sort order --
			query = SortBy switch
			{
				SortOrder.NewestFirst => query.OrderByDescending(t => t.Created),
				SortOrder.OldestFirst => query.OrderBy(t => t.Created),
				_ => query
			};

			// -- Timestamp --
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

			return query;
		}
	}
}