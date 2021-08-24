using System;
using Forum.Entity;

namespace Forum.Model
{
	public class SearchQueryBuilder
	{
		private SearchQuery _searchQuery;

		/// <summary>
		/// Constructor
		/// </summary>
		public SearchQueryBuilder()
		{
			_searchQuery = new SearchQuery();
		}
		
		///// <summary>
		///// The constructed query.
		///// </summary>
		//public IEnumerable<Thread> GetQuery => ConstructQuery();

		/// <summary>
		/// Query a title.
		/// </summary>
		/// <param name="title">Title to query.</param>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder AddQueryTitle(string title)
		{
			_searchQuery.TitleStrings.Add(title);
			
			return this;
		}

		/// <summary>
		/// Query a tag.
		/// </summary>
		/// <param name="tag">Tag to query.</param>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder AddQueryTag(Tag tag)
		{
			_searchQuery.Tags.Add(tag);
			
			return this;
		}

		/// <summary>
		/// Query a user.
		/// </summary>
		/// <param name="user">User to query.</param>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder AddQueryUser(User user)
		{
			_searchQuery.Users.Add(user);
			
			return this;
		}

		/// <summary>
		/// Sort the queried elements by newest first.
		/// </summary>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder SortByNewest()
		{
			_searchQuery.SortBy = SearchQuery.SortOrder.NewestFirst;
			
			return this;
		}

		public SearchQueryBuilder SortBy(SearchQuery.SortOrder sortBy)
		{
			_searchQuery.SortBy = sortBy;

			return this;
		}

		/// <summary>
		/// Sort the queried elements by oldest first.
		/// </summary>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder SortByOldest()
		{
			_searchQuery.SortBy = SearchQuery.SortOrder.OldestFirst;
			
			return this;
		}

		/// <summary>
		/// Query only saved threads.
		/// </summary>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder SavedOnly(User user)
		{
			_searchQuery.SavedByUser = user;
			
			return this;
		}

		/// <summary>
		/// Query elements before/after (depending on whether sorting by newest- or oldest-first) timestamp.
		/// </summary>
		/// <param name="timestamp">Timestamp</param>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder StartAtTimestamp(DateTime timestamp)
		{
			_searchQuery.TimeStamp = timestamp;
			
			return this;
		}

		/// <summary>
		/// Add your own query in form of a delegate.
		/// </summary>
		/// <param name="advancedQuery">Delegate query</param>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder AddAdvancedQuery(SearchQuery.AdvancedQuery advancedQuery)
		{
			_searchQuery.AdvancedQueries.Add(advancedQuery);
			
			return this;
		}

		/// <summary>
		/// Build the search query.
		/// </summary>
		/// <returns>Build search query.</returns>
		public SearchQuery Build()
		{
			return _searchQuery;
		}

		/// <summary>
		/// Reset the builder.
		/// </summary>
		/// <returns>The query-builder object itself.</returns>
		public SearchQueryBuilder Reset()
		{
			// Nothing to dispose.
			_searchQuery = new SearchQuery();
		
			return this;
		}
	}
}