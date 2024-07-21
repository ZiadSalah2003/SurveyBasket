using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
		public AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions)
		{
			_authService = authService;
			_jwtOptions = jwtOptions.Value;
		}
		[HttpPost("")]
		public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
		{
			var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
			return authResult is null ? BadRequest("Invalid Email Or Password") : Ok(authResult);

		}
		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
		{
			var authResult = await _authService.GetTokenAsync(request.Token, request.RefreshToken, cancellationToken);
			return authResult is null ? BadRequest("Invalid Token") : Ok(authResult);

		}

		[HttpPost("revoke-refresh-token")]
		public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
		{
			var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
			return isRevoked ? Ok() : BadRequest("Operation Failed");

		}
	}
}
