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
		public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
		{
			var user = await _userManger.FindByEmailAsync(email);
			if (user is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidCredentails);
			var userPassword = await _userManger.CheckPasswordAsync(user, password);
			if (!userPassword)
				return Result.Failure<AuthResponse>(UserErrors.InvalidCredentails);
			var (token, expiresIn) = _jwtProvider.GenerateToken(user);

			var refreshToken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

			user.RefreshTokens.Add(new RefreshToken
			{
				Token = refreshToken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManger.UpdateAsync(user);
			var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
			return Result.Success(response);
		}
		public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
			var user = await _userManger.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
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
			var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);
			return Result.Success(response);
		}
		public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			var user = await _userManger.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			userRefreshToken.RevokedOn = DateTime.UtcNow;
			await _userManger.UpdateAsync(user);

			return Result.Success();
		}

		private static string GenerateRefreshToken()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		}

	}
}
