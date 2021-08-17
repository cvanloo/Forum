using System.Threading.Tasks;

namespace Forum.Data
{
	public interface IMailService
	{
		/// <summary>
		/// Send an email.
		/// </summary>
		/// <param name="message">Email to send.</param>
		/// <returns></returns>
		Task SendAsync(Model.Message message);
	}
}
