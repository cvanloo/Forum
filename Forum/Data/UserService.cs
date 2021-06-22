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

			if (!BCrypt.Net.BCrypt.Verify(password, user.PwHash))
			{
				throw new Exception("Wrong password.");
			}

			return user;
		}
	}
}
