using System.Threading.Tasks;

namespace Forum.Data
{
	public interface IMailService
	{
		Task Send(Model.Message message);
	}
}
