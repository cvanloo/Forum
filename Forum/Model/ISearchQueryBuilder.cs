namespace Forum.Model
{
	public interface ISearchQueryBuilder
	{
		public SearchQuery Build();
		public SearchQueryBuilder Reset();
	}
}