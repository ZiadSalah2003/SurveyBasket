using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Contracts.cs.Authentication;
using SurveyBasket.API.Entities;
using System.Data;
using System.Security.Cryptography;

namespace SurveyBasket.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManger;
		private readonly IJwtProvider _jwtProvider;
		private readonly int _refreshTokenExpiryDays = 14;

		public AuthService(UserManager<ApplicationUser> userManger, IJwtProvider jwtProvider)
		{
			_userManger = userManger;
			_jwtProvider = jwtProvider;
		}
		public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			var user = await _userManger.FindByEmailAsync(email);
			if (user is null)
				return null;
			var userPassword = await _userManger.CheckPasswordAsync(user, password);
			if (!userPassword)
				return null;
			var (token, expiresIn) = _jwtProvider.GenerateToken(user);

			var refreshToken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

			user.RefreshTokens.Add(new RefreshToken
			{
				Token = refreshToken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManger.UpdateAsync(user);

			return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
		}
		public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return null;
			var user = await _userManger.FindByIdAsync(userId);
			if (user is null)
				return null;
			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return null;
			userRefreshToken.RevokedOn = DateTime.UtcNow;

			var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

			var newRefreshToken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

			user.RefreshTokens.Add(new RefreshToken
			{
				Token = newRefreshToken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManger.UpdateAsync(user);
			return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);
		}
		public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return false;
			var user = await _userManger.FindByIdAsync(userId);
			if (user is null)
				return false;
			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return false;
			userRefreshToken.RevokedOn = DateTime.UtcNow;
			await _userManger.UpdateAsync(user);
			return true;
		}

		private static string GenerateRefreshToken()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		}

	}
}
