using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Data
{
	public interface IMailService
	{
		Task Send(Model.Message message);
	}
}
