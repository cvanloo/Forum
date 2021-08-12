using System;
using System.Collections.Generic;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Module
{
	public class Search
	{
		private readonly Model.Database _dbContext;

		public enum SortOrder
		{
			NewestFirst,
			OldestFirst
		}

		public DateTime LastTimeStamp { get; set; } = DateTime.Now;
		public SortOrder SortBy { get; set; }

		public Search(Model.Database dbContext)
		{
			_dbContext = dbContext;
		}

		public List<Thread> GetThreads()
		{
			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Created.CompareTo(LastTimeStamp) < 0)
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Created.CompareTo(LastTimeStamp) > 0)
				.Take(50).ToList();
		}

		public List<Thread> SearchThreadsByTitle(string title)
		{
			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Title.Contains(title) && t.Created.CompareTo(LastTimeStamp) < 0)
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Title.Contains(title) && t.Created.CompareTo(LastTimeStamp) > 0)
				.Take(50).ToList();
		}

		public List<Thread> SearchThreadsByTag(IEnumerable<Tag> tags)
		{
			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					/* EF Core usually tries to evaluate as much as possible of a query on the server
					 * (it translates the query into a database query, and lets the database do all the work).
					 * This is not possible for some queries, like `Intersect()` (The db doesn't know how
					 * intersect is implemented) below. Instead, EF has to evaluate the query by itself, on
					 * the client (C# code). This can lead to poor performance, in that case EF throws a runtime
					 * exception. To prevent this and enforce client evaluation, `AsEnumerable()` can be used.
					 * https://docs.microsoft.com/en-us/ef/core/querying/client-eval
					 * TODO: This is _not_ a good idea
					 */
					.AsEnumerable()
					.Where(t => tags.Intersect(t.Tags).Any() && t.Created.CompareTo(LastTimeStamp) < 0)
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.AsEnumerable()
				.Where(t => tags.Intersect(t.Tags).Any() && t.Created.CompareTo(LastTimeStamp) > 0)
				.Take(50).ToList();
		}

		public List<Thread> SearchThreadsByUser(string name)
		{
			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Creator.AccountName.Contains(name) && t.Created.CompareTo(LastTimeStamp) < 0)
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Creator.AccountName.Contains(name) && t.Created.CompareTo(LastTimeStamp) > 0)
				.Take(50).ToList();
		}
	}
}