using System;
using System.Collections.Generic;
using System.Linq;
using Forum.Data;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Model
{
	// !!! Don't use this class !!!
	// ~~ Use Forum.Model.SearchQuery instead ~~
/*
	           ...
             ;::::;
           ;::::; :;
         ;:::::'   :;
        ;:::::;     ;.
       ,:::::'       ;           OOO\
       ::::::;       ;          OOOOO\
       ;:::::;       ;         OOOOOOOO
      ,;::::::;     ;'         / OOOOOOO
    ;:::::::::`. ,,,;.        /  / DOOOOOO
  .';:::::::::::::::::;,     /  /     DOOOO
 ,::::::;::::::;;;;::::;,   /  /        DOOO
;`::::::`'::::::;;;::::: ,#/  /          DOOO
:`:::::::`;::::::;;::: ;::#  /            DOOO
::`:::::::`;:::::::: ;::::# /              DOO
`:`:::::::`;:::::: ;::::::#/               DOO
 :::`:::::::`;; ;:::::::::##                OO
 ::::`:::::::`;::::::::;:::#                OO
 `:::::`::::::::::::;'`:;::#                O
  `:::::`::::::::;' /  / `:#
   ::::::`:::::;'  /  /   `#
 */
	
	// TODO: OrderBy and OrderByDescending -> Just one sort?
	// TODO: Compare to -> Just one comparison, maybe in sort?
	// TODO: Always just pass the id instead of the whole user, when we need to retrieve the whole object form
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
		public List<Thread> FindThreads()
		{
			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Created.CompareTo(LastTimeStamp) < 0)
					.AsNoTracking() // We're not going to do any changes to the elements, no need to track them.
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
		public List<Thread> FindThreadsByTitle(string title)
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
		/// <param name="tagID">Id of the tag to search</param>
		/// <returns>Found threads</returns>
		public List<Thread> FindThreadsByTag(int tagID)
		{
			// TODO: Do we need to care about how many post it gets from the database or is ef core smart enough?
			var tag = _dbContext.Tags.First(t => t.Id == tagID);

			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Tags.Contains(tag) && t.Created.CompareTo(LastTimeStamp) < 0)
					.AsNoTracking()
					.ToList();
			}

			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Tags.Contains(tag) && t.Created.CompareTo(LastTimeStamp) > 0)
				.AsNoTracking()
				.ToList();


			//foundThreads.Sort((t1, t2) =>
			//	SortBy == SortOrder.NewestFirst
			//		? t2.Created.CompareTo(t1.Created)
			//		: t1.Created.CompareTo(t2.Created));
		}

		/// <summary>
		/// Retrieve next 50 threads from user.
		/// </summary>
		/// <param name="userID">Id of user</param>
		/// <returns>Found threads</returns>
		public List<Thread> FindThreadsByUser(int userID)
		{
			var user = _dbContext.Users.First(u => u.Id == userID);

			if (SortOrder.NewestFirst == SortBy)
			{
				return _dbContext.Threads
					.Include(t => t.Creator)
					.Include(t => t.Forum)
					.Include(t => t.Tags)
					.OrderByDescending(t => t.Created)
					.Where(t => t.Creator == user && t.Created.CompareTo(LastTimeStamp) < 0)
					.AsNoTracking()
					.Take(50).ToList();
			}
			
			return _dbContext.Threads
				.Include(t => t.Creator)
				.Include(t => t.Forum)
				.Include(t => t.Tags)
				.OrderBy(t => t.Created)
				.Where(t => t.Creator == user && t.Created.CompareTo(LastTimeStamp) > 0)
				.AsNoTracking()
				.Take(50).ToList();
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