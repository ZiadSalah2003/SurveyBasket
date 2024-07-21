using Microsoft.AspNetCore.Identity.Data;
using SurveyBasket.API.Contracts.cs.Authentication;

namespace SurveyBasket.API.Services
{
	public interface IAuthService
	{
		Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
		Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
		Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
		//Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
	}
}
