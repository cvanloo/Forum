using Forum.Entity;

namespace Forum.Data
{
	public interface IUserService
	{
		User ValidateUser(string username, string password);
		string StoreSessionToken(User user);
		User GetUserFromSessionToken(string token);
		void RemoveSession(User user);
	}
}
