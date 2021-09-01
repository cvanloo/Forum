using System.Collections.Generic;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data
{
	public class ChatService : IChatService
	{
		private readonly IDbContextFactory<Model.Database> _dbContextFactory;
		private readonly List<IChatService.MsgHandler> _msgHandlers = new();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContextFactory">Database factory</param>
		public ChatService(IDbContextFactory<Model.Database> dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}
		
		/// <inheritdoc cref="IChatService.FindChat"/>
		public Chat FindChat(User user1, User user2)
		{
			using var dbContext = _dbContextFactory.CreateDbContext();
			
			var p1 = dbContext.Users.First(u => u.Id == user1.Id);
			var p2 = dbContext.Users.First(u => u.Id == user2.Id);

			if (p1 == p2) return null;

			return dbContext.Chats.FirstOrDefault(c => c.Participants.Contains(p1) && c.Participants.Contains(p2));
		}

		/// <inheritdoc cref="IChatService.Subscribe"/>
		public void Subscribe(IChatService.MsgHandler msgHandler)
		{
			_msgHandlers.Add(msgHandler);
		}

		/// <inheritdoc cref="IChatService.Unsubscribe"/>
		public void Unsubscribe(IChatService.MsgHandler msgHandler)
		{
			_msgHandlers.Remove(msgHandler);
		}

		/// <inheritdoc cref="IChatService.GetMessages"/>
		public ICollection<ChatMessage> GetMessages(Chat chat)
		{
			using var dbContext = _dbContextFactory.CreateDbContext();
			
			return dbContext.Chats
				.Include(c => c.Messages)
					.ThenInclude(m => m.Sender)
				.First(c => c.Id == chat.Id).Messages;
		}

		/// <inheritdoc cref="IChatService.SendMessage"/>
		public void SendMessage(Chat chat, ChatMessage message)
		{
			using var dbContext = _dbContextFactory.CreateDbContext();

			var localChat = dbContext.Chats
				.Include(c => c.Messages)
				.First(c => c.Id == chat.Id);
			localChat.Messages.Add(message);
			dbContext.SaveChanges();
			Notify(message);
		}

		/// <summary>
		/// Notifies all subscribers of the new message.
		/// </summary>
		/// <param name="message">New Message</param>
		private void Notify(ChatMessage message)
		{
			foreach (var sub in _msgHandlers.Where(sub => sub.Chat.Id == message.Chat.Id))
			{
				sub.Handler(message);
			}
		}
	}
}