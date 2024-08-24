using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.API.Helpers;
using System.Text;

namespace SurveyBasket.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManger;
		private readonly IJwtProvider _jwtProvider;
		private readonly int _refreshTokenExpiryDays = 14;
		private readonly ILogger<AuthService> _logger;
		private readonly IEmailSender _emailSender;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(UserManager<ApplicationUser> userManger, IJwtProvider jwtProvider, ILogger<AuthService> logger, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
		{
			_userManger = userManger;
			_jwtProvider = jwtProvider;
			_logger = logger;
			_emailSender = emailSender;
			_httpContextAccessor = httpContextAccessor;
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

		public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
		{
			var emailIsExist = await _userManger.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
			if (emailIsExist)
				return Result.Failure(UserErrors.DuplicatedEmail);
			var user = request.Adapt<ApplicationUser>();
			user.UserName = request.Email;

			var result = await _userManger.CreateAsync(user, request.Password);
			if (result.Succeeded)
			{
				var code = await _userManger.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				_logger.LogInformation("Confirmation Code {code}", code);

				await SendConfirmationEmailAsync(user, code);

				return Result.Success();
			}
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}

		public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
		{
			var user = await _userManger.FindByIdAsync(request.UserId);
			if (user is null)
				return Result.Failure(UserErrors.InvalidCode);

			if (user.EmailConfirmed)
				return Result.Failure(UserErrors.DuplicatedConfirmation);

			var code = request.Code;
			try
			{
				code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			}
			catch (FormatException)
			{
				return Result.Failure(UserErrors.InvalidCode);
			}
			var result = await _userManger.ConfirmEmailAsync(user, code);
			if (result.Succeeded)
			{
				return Result.Success();
			}
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}

		public async Task<Result> ResendConfirmEmailAsync(ResendConfirmationEmailRequest request)
		{
			var user = await _userManger.FindByEmailAsync(request.Email);
			if (user is null)
				return Result.Success();

			if (user.EmailConfirmed)
				return Result.Failure(UserErrors.DuplicatedConfirmation);

			var code = await _userManger.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			_logger.LogInformation("Confirmation Code {code}", code);

			await SendConfirmationEmailAsync(user, code);

			return Result.Success();
		}
		private static string GenerateRefreshToken()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		}

		private async Task SendConfirmationEmailAsync(ApplicationUser user, string code)
		{
			var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

			var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
				new Dictionary<string, string>
				{
					{"{{name}}",user.FirstName },
					{ "{{action_url}}",$"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
				}
			);
			BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survay Basket: Confirm your email", emailBody));
			
			await Task.CompletedTask;
		}
	}
}
