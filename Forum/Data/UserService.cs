using Forum.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Model;

namespace Forum.Data
{
	public class UserService : IUserService
	{
		private readonly IDbContextFactory<Database> _dbContext;

		public UserService(IDbContextFactory<Database> dbContext)
		{
			_dbContext = dbContext;
		}

		public User ValidateUser(string username, string password)
		{
			using var db = _dbContext.CreateDbContext();
			
			User user = db.Users.Where(u => u.AccountName == username).FirstOrDefault();

			if (null == user)
			{
				throw new Exception("User not found.");
			}
			
			if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.PwHash))
			{
				throw new Exception("Wrong password.");
			}

			return user;
		}

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

		public User GetUserFromSessionToken(string token)
		{
			using var db = _dbContext.CreateDbContext();

			Session session = db.Sessions.Include(s => s.Owner).Where(s => s.Identifier == "SESSION_ID" && s.Value == token).FirstOrDefault();

			return session.Owner;
		}

		public void RemoveSession(User user)
		{
			using var db = _dbContext.CreateDbContext();

			Session[] sessions = db.Sessions.Where(s => s.Owner == user).ToArray();

			db.Sessions.RemoveRange(sessions);
			
			db.SaveChanges();
		}
	}
}
