using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.API.Settings;

namespace SurveyBasket.API.Health
{
	public class MailProviderHealthCheck(IOptions<MailSettings> maillSettins) : IHealthCheck
	{
		private readonly MailSettings _maillSettins = maillSettins.Value;
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				using var smtp = new SmtpClient();

				smtp.Connect(_maillSettins.Host, _maillSettins.Port, SecureSocketOptions.StartTls, cancellationToken);
				smtp.Authenticate(_maillSettins.Mail, _maillSettins.Password, cancellationToken);
			
				return await Task.FromResult(HealthCheckResult.Healthy());
			}
			catch(Exception exception)
			{
				return await Task.FromResult(HealthCheckResult.Unhealthy(exception: exception));
			}
		}
	}
}
