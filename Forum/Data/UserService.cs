using Forum.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Model;
using Microsoft.Extensions.Configuration;

namespace Forum.Data
{
	public class UserService : IUserService
	{
		private readonly IDbContextFactory<Database> _dbContext;

		public UserService(IDbContextFactory<Database> dbContext)
		{
			_dbContext = dbContext;
		}

		/// <summary>
		/// Validates `username` and `password`. Throws an Exception if no user
		/// is found or if the password doesn't match, else it returns the user
		/// on success.
		/// </summary>
		/// <param name="username">Username of the user to log in.</param>
		/// <param name="password">Password of the user to log in.</param>
		/// <returns>
		/// The User entity on success. Will throw an exception otherwise.
		/// </returns>
		public User ValidateUser(string identifier, string password)
		{
			using var db = _dbContext.CreateDbContext();

			User user;

			if (identifier.Contains('@'))
			{
				user = db.Users.Where(u => u.Email == identifier).FirstOrDefault();
			}
			else
			{
				user = db.Users.Where(u => u.AccountName == identifier).FirstOrDefault();
			}

			if (null == user)
			{
				throw new Exception("User not found.");
			}
			
			if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.PwHash))
			{
				throw new Exception("Wrong password.");
			}

			// TODO: Configure workFactor at one place
			if (BCrypt.Net.BCrypt.PasswordNeedsRehash(user.PwHash, 12))
			{
				user.PwHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);
				db.SaveChanges();
			}

			return user;
		}

		/// <summary>
		/// Stores a session token in the servers database.
		/// Leftovers from previous sessions of the same user will be deleted.
		/// </summary>
		/// <param name="user">The owner of the session.</param>
		/// <returns>The session token.</returns>
		public string StoreSessionToken(User user)
		{
			RemoveSession(user);

			using var db = _dbContext.CreateDbContext();

			string rndToken = Guid.NewGuid().ToString();

			Session session = new Session()
			{
				UserId = user.Id,
				Identifier = "SESSION_ID",
				Value = rndToken
			};

			db.Sessions.Add(session);
			db.SaveChanges();

			return rndToken;
		}

		/// <summary>
		/// Searches for an existing session in the db, based on the token.
		/// </summary>
		/// <param name="token">The session token.</param>
		/// <returns>If a session was found its owner, else `null`.</returns>
		public User GetUserFromSessionToken(string token)
		{
			using var db = _dbContext.CreateDbContext();

			Session session = db.Sessions.Include(s => s.User).Where(s => s.Identifier == "SESSION_ID" && s.Value == token).FirstOrDefault();

			return session?.User;
		}

		/// <summary>
		/// Deletes a users session.
		/// </summary>
		/// <param name="user">The session owner.</param>
		public void RemoveSession(User user)
		{
			using var db = _dbContext.CreateDbContext();

			Session[] sessions = db.Sessions.Where(s => s.User == user).ToArray();

			db.Sessions.RemoveRange(sessions);
			
			db.SaveChanges();
		}
	}
}
