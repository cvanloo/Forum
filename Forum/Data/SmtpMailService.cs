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
				/* Delivery via SMTP */
				//smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				//
				//var credentials = new NetworkCredential()
				//{
				//	UserName = "name@email.ch",
				//	Password = "password"
				//};
				//
				//smtp.Credentials = credentials;
				//smtp.Host = "host";
				//smtp.Port = 587;
				//smtp.EnableSsl = true;

				/* Dump mail to a local directory */
				smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				smtp.PickupDirectoryLocation = @"C:\tmp\mail";

				MailMessage mail = new MailMessage()
				{
					Body = message.Body,
					Subject = message.Subject,
					From = new MailAddress(message.From)
				};
				mail.To.Add(string.Join(',', message.To));

				await smtp.SendMailAsync(mail);
			}
		}
	}
}
