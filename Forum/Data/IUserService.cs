using Forum.Entity;

namespace Forum.Data
{
	public interface IUserService
	{
		/// <summary>
		/// Get user from database and check credentials.
		/// </summary>
		/// <param name="identifier">Accountname or Email address</param>
		/// <param name="password">Password</param>
		/// <returns>If the credentials are valid the user is returned, else null.</returns>
		User ValidateUser(string identifier, string password);
		
		/// <summary>
		/// Store session token in database.
		/// </summary>
		/// <param name="user">Owner of session</param>
		/// <returns>Session token</returns>
		string StoreSessionToken(User user);
		
		/// <summary>
		/// Get user from session.
		/// </summary>
		/// <param name="token">Session token</param>
		/// <returns>The user, if session exist in database, else null.</returns>
		User GetUserFromSessionToken(string token);
		
		/// <summary>
		/// Delete session(s) from database.
		/// </summary>
		/// <param name="user">The users which sessions should be deleted.</param>
		void RemoveSession(User user);
	}
}
