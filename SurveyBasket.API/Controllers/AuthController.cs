using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Services;

namespace SurveyBasket.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly JwtOptions _jwtOptions;
		public AuthController(IAuthService authService,IOptions<JwtOptions> jwtOptions)
		{
			_authService = authService;
			_jwtOptions = jwtOptions.Value;
		}
		[HttpPost("")]
		public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
		{
			var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
			 return authResult is null ? BadRequest("Invalid Email Or Password") :Ok(authResult);

		}
	}
}
