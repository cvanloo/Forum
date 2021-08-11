﻿namespace Forum.Data
{
	public delegate void MessageEventHandler();

	public class InfoMessage
	{
		public event MessageEventHandler MessageEvent;
		private string _message;

		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				MessageEvent?.Invoke();
			}
		}
	}
}
