using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBasket.API.Abstractions;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Contracts.cs.Authentication;
using SurveyBasket.API.Services;

namespace SurveyBasket.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly JwtOptions _jwtOptions;
		private readonly ILogger<AuthController> _logger;
		public AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions, ILogger<AuthController> logger)
		{
			_authService = authService;
			_jwtOptions = jwtOptions.Value;
			_logger = logger;
		}
		[HttpPost("")]
		public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Logging with email: {email} and password:{passwrod}", request.Email, request.Password);
			var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
			return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
		}
		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
		{
			var result = await _authService.GetTokenAsync(request.Token, request.RefreshToken, cancellationToken);
			return result.IsSuccess
				? Ok()
				: Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
		}

		[HttpPost("revoke-refresh-token")]
		public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
		{
			var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
			return result.IsSuccess
				? Ok()
				: Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);

		}
	}
}
