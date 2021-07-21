using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Forum.Data
{
	public class SmtpMailService : IMailService
	{
		public async Task Send(Model.Message message)
		{
			using (var smtp = new SmtpClient())
			{
				smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				smtp.PickupDirectoryLocation = @"C:\tmp\mail";

				MailMessage mail = new MailMessage()
				{
					Body = message.Body,
					Subject = message.Subject,
					From = new MailAddress(message.From)
				};
				mail.To.Add(string.Join(';', message.To));

				await smtp.SendMailAsync(mail);
			}
		}
	}
}
