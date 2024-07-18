using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.API.Services;

namespace SurveyBasket.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}
		[HttpPost("")]
		public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
		{
			var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
			if (authResult is null)
				return BadRequest("Invalid Email Or Password");
			return Ok(authResult);
		}
	}
}
