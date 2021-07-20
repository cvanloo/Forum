using System;

namespace Forum.Data
{
	public delegate void CommentEventHandler();
	
	public class CommentService
	{
		public event CommentEventHandler CommentEvent;

		public void RaiseCommentEvent()
		{
			CommentEvent?.Invoke();
		}
	}
}
