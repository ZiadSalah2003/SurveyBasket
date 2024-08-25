using Microsoft.AspNetCore.Identity.Data;
using SurveyBasket.API.Contracts.cs.Authentication;

namespace SurveyBasket.API.Services
{
	public interface IAuthService
	{
		Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
		Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
		Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
		Task<Result> RegisterAsync(Contracts.cs.Authentication.RegisterRequest request, CancellationToken cancellationToken = default);
		Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
		Task<Result> ResendConfirmEmailAsync(Contracts.cs.Authentication.ResendConfirmationEmailRequest request);
		Task<Result> SendResetPasswordCodeAsync(string email);
		Task<Result> ResetPasswordAsync(Contracts.cs.Authentication.ResetPasswordRequest request);
	}
}
