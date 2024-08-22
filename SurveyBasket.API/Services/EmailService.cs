using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.API.Settings;


namespace SurveyBasket.API.Services
{
	public class EmailService(IOptions<MailSettings> maillSettins, ILogger<EmailService> logger) : IEmailSender
	{
		private readonly MailSettings _maillSettins = maillSettins.Value;
		private readonly ILogger<EmailService> _logger= logger;

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var message = new MimeMessage
			{
				Sender=MailboxAddress.Parse(_maillSettins.Mail),
				Subject=subject
			};
			message.To.Add(MailboxAddress.Parse(email));

			var builder = new BodyBuilder
			{
				HtmlBody=htmlMessage
			};

			message.Body = builder.ToMessageBody();

			using var smtp=new SmtpClient();

			_logger.LogInformation("Sending email to {email}", email);

			smtp.Connect(_maillSettins.Host, _maillSettins.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_maillSettins.Mail, _maillSettins.Password);
			await smtp.SendAsync(message);
			smtp.Disconnect(true);
		}
	}
}
