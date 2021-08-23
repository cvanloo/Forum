using System.Collections.Generic;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Model
{
	public class SearchQuery
	{
		private List<string> _titleStrings = new();
		private List<Tag> _tags = new();
		private List<User> _users = new();
		
		/// <summary>
		/// The constructed query.
		/// </summary>
		//public IEnumerable<Thread> GetQuery => ConstructQuery();

		/// <summary>
		/// Add a title to the search query.
		/// </summary>
		/// <param name="title">Title to add to the query.</param>
		/// <returns>The query object itself.</returns>
		public SearchQuery AddSearchTitle(string title)
		{
			_titleStrings.Add(title);
			
			return this;
		}

		/// <summary>
		/// Add a tag to the query.
		/// </summary>
		/// <param name="tag">The tag to add to the query.</param>
		/// <returns>The query object itself.</returns>
		public SearchQuery AddSearchTag(Entity.Tag tag)
		{
			_tags.Add(tag);
			
			return this;
		}

		/// <summary>
		/// Add a user to the query.
		/// </summary>
		/// <param name="user">User to add to the query.</param>
		/// <returns>The query object itself.</returns>
		public SearchQuery AddSearchUser(Entity.User user)
		{
			_users.Add(user);
			
			return this;
		}

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
				.Include(t => t.Tags);

			// TODO(1): _Or_ not _and_
			// TODO(2): Sort order
			foreach (var title in _titleStrings)
			{
				query = query.Where(t => t.Title.Contains(title));
			}

			foreach (var tag in _tags)
			{
				query = query.Where(t => t.Tags.Contains(tag));
			}

			foreach (var user in _users)
			{
				query = query.Where(t => t.Creator == user);
			}

			return query;
		}
	}
}