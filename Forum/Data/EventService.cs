using System;

namespace Forum.Data
{
	//public delegate void CommentEventHandler();
	
	public class EventService
	{
		//public event CommentEventHandler ChangedEvent;
		public EventHandler ChangedEvent;

		public void UpdateStatus()
		{
			//ChangedEvent?.Invoke();
			ChangedEvent?.Invoke(null, EventArgs.Empty);
		}
	}
}
