using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
		public IEnumerable<Thread> Construct(Database dbContext)
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
			{
				var predicate = PredicatesBuilder.False<Thread>();
				foreach (var title in TitleStrings)
				{
					predicate = predicate.Or(t => t.Title.ToLower().Contains(title.ToLower()));
				}

				query = query.Where(predicate);
			}

			// Tags
			if (Tags.Any())
			{
				var predicate = PredicatesBuilder.False<Thread>();
				foreach (var tag in Tags)
				{
					predicate = predicate.Or(t => t.Tags.Contains(tag));
				}

				query = query.Where(predicate);
			}

			// Users
			if (Users.Any())
			{
				var predicate = PredicatesBuilder.False<Thread>();
				foreach (var user in Users)
				{
					predicate = predicate.Or(t => t.Creator == user);
				}

				query = query.Where(predicate);
			}

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

	public static class PredicatesBuilder
	{
		/// <summary>
		/// Default to `false`.
		/// </summary>
		/// <typeparam name="T">Type of object to compare.</typeparam>
		/// <returns>The predicate.</returns>
		public static Func<T, bool> False<T>()
		{
			return f => false;
		}

		/// <summary>
		/// Default to `true`.
		/// </summary>
		/// <typeparam name="T">Type of object to compare.</typeparam>
		/// <returns>The predicate.</returns>
		public static Func<T, bool> True<T>()
		{
			return f => true;
		}
		
		/// <summary>
		/// Predicate `or` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>The predicate.</returns>
		public static Func<T, bool> Or<T>(this Func<T, bool> left, Func<T, bool> right)
		{
			return t => left(t) || right(t);
		}
	}
}