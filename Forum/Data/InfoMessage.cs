namespace Forum.Data
{
	public delegate void MessageEventHandler();

	public class InfoMessage
	{
		public event MessageEventHandler MessageEvent;
		private string _message;

		/// <summary>
		/// Set the message to display.
		/// All subscribers will be notified of the change.
		/// </summary>
		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				MessageEvent?.Invoke();
			}
		}
	}
}
