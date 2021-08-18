using System.Collections.Generic;

namespace Forum.Data
{
	public interface IChatService
	{
		public delegate void NewMessageHandler(Entity.ChatMessage newMessage);
		
		public struct MsgHandler
		{
			/// <summary>
			/// Chat to subscribe to.
			/// </summary>
			public Entity.Chat Chat;
			
			/// <summary>
			/// Method to be called when chat is updated.
			/// </summary>
			public NewMessageHandler Handler;
		}
		
		/// <summary>
		/// Get chat between two users.
		/// </summary>
		/// <param name="user1">One participant</param>
		/// <param name="user2">Other participant</param>
		/// <returns>Found chat or null.</returns>
		public Entity.Chat FindChat(Entity.User user1, Entity.User user2);
		
		/// <summary>
		/// Subscribe to get notified when a new message is sent.
		/// </summary>
		/// <param name="newMsgHandler">
		/// The subscription, consists of the chat to subscribe to an a method that will be called whenever a new
		/// message is sent to the chat.
		/// </param>
		public void Subscribe(MsgHandler newMsgHandler);
		
		/// <summary>
		/// Unsubscribe event handler.
		/// </summary>
		/// <param name="newMsgHandler">The subscription to remove</param>
		public void Unsubscribe(MsgHandler newMsgHandler);
		
		/// <summary>
		/// Retrieves all chat messages
		/// </summary>
		/// <param name="chat">Chat</param>
		/// <returns>Collection of chat messages.</returns>
		public ICollection<Entity.ChatMessage> GetMessages(Entity.Chat chat);
		
		/// <summary>
		/// Send a message to the chat.
		/// </summary>
		/// <param name="chat">Chat to send message to.</param>
		/// <param name="message">Message to be sent.</param>
		public void SendMessage(Entity.Chat chat, Entity.ChatMessage message);
	}
}