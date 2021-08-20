using System;
using System.Collections.Generic;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Model
{
	public class Search : IDisposable
	{
		private readonly Database _dbContext;

		public enum SortOrder
		{
			NewestFirst,
			OldestFirst
		}

		public DateTime LastTimeStamp { get; init; } = DateTime.Now;
		public SortOrder SortBy { get; init; } // Setter initialization only "... new Module.Search() { SortBy = ... };"

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">Database context</param>
		public Search(Database dbContext)
		{
			_dbContext = dbContext;
		}

		/// <summary>
		/// Retrieve next 50 threads.
		/// </summary>
		/// <returns>Found threads</returns>
		public List<Thread> GetThreads()
		{
			// TODO: OrderBy and OrderByDescending -> Just one sort?
			// TODO: Compare to -> Just one comparison, maybe in sort?
			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Created.CompareTo(LastTimeStamp) < 0)
					.AsNoTracking()
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Created.CompareTo(LastTimeStamp) > 0)
				.AsNoTracking()
				.Take(50).ToList();
		}

		/// <summary>
		/// Retrieve next 50 threads that match title.
		/// </summary>
		/// <param name="title">Thread title</param>
		/// <returns>Found threads</returns>
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
					.AsNoTracking()
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Title.Contains(title) && t.Created.CompareTo(LastTimeStamp) > 0)
				.AsNoTracking()
				.Take(50).ToList();
		}

		/// <summary>
		/// Retrieve next 50 threads that contain (one of) the tags.
		/// </summary>
		/// <param name="tags">Tags to search for</param>
		/// <returns>Found threads</returns>
		public List<Thread> SearchThreadsByTag(List<Tag> tags)
		{
			// TODO: Do we need to care about how many post it gets from the database or is ef core smart enough?
			List<Thread> foundThreads = new();
			foreach (var tag in tags)
			{
				foundThreads.AddRange(
					_dbContext.Threads
						.Include(t => t.Creator)
						.Include(t => t.Forum)
						.Include(t => t.Tags)
						.Where(t => t.Tags.Contains(tag) && (SortBy == SortOrder.NewestFirst
							? t.Created.CompareTo(LastTimeStamp) < 0
							: t.Created.CompareTo(LastTimeStamp) > 0))
						.AsNoTracking()
				);
			}

			// Bc we get the _threads per tag_ and add them together in a list we need to sort the list
			// to get the threads in order again.
			foundThreads.Sort((t1, t2) =>
				SortBy == SortOrder.NewestFirst
					? t2.Created.CompareTo(t1.Created)
					: t1.Created.CompareTo(t2.Created));

			return foundThreads.Take(50).ToList();
		}

		/// <summary>
		/// Retrieve next 50 threads from user.
		/// </summary>
		/// <param name="name">Accountname of user</param>
		/// <returns>Found threads</returns>
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
					.AsNoTracking()
					.Take(50).ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Creator.AccountName.Contains(name) && t.Created.CompareTo(LastTimeStamp) > 0)
				.AsNoTracking()
				.Take(50).ToList();
		}

		// TODO: Always just pass the id instead of the whole user, when we need to retrieve the whole object form
		// the database again anyway.
		public List<Thread> GetSavedThreads(int userId)
		{
			var user = _dbContext.Users
				.Include(u => u.SavedThreads)
				.ThenInclude(t => t.Creator)
				.Include(u => u.SavedThreads)
				.ThenInclude(t => t.Forum)
				.Include(u => u.SavedThreads)
				.ThenInclude(t => t.Tags)
				.AsNoTracking()
				.First(u => u.Id == userId);

			return user.SavedThreads.Take(50).ToList();
		}
		
		/// <summary>
		/// Dispose: Clean up resources.
		/// </summary>
		public void Dispose()
		{
			_dbContext?.Dispose();
		}
	}
}