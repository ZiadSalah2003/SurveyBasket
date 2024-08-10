using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.API.Errors
{
	public class GlobalExceptionHandler : IExceptionHandler
	{
		private readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
		{
			_logger = logger;
		}

		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			_logger.LogError(exception, "Something went wrong {Message}", exception.Message);
			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "An error occurred while processing your request",
				Detail = "An error occurred while processing your request"
			};
			httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
			return true;
		}
	}
}
