using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Contracts.cs.Authentication;
using System.Data;

namespace SurveyBasket.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _identityUser;
		private readonly IJwtProvider _jwtProvider;

		public AuthService(UserManager<ApplicationUser> identityUser, IJwtProvider jwtProvider)
		{
			_identityUser = identityUser;
			_jwtProvider = jwtProvider;
		}
		public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			var user = await _identityUser.FindByEmailAsync(email);
			if (user is null)
				return null;
			var userPassword = await _identityUser.CheckPasswordAsync(user, password);
			if (!userPassword)
				return null;
			var (token, expiresIn) = _jwtProvider.GenerateToken(user);
			return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn);
		}

		public Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
