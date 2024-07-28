namespace SurveyBasket.API.Abstractions
{
	public static class ResultExtensions
	{
		public static ObjectResult ToProblem(this Result result,int statusCode,string title)
		{
			if(result.IsSuccess)
				throw new InvalidOperationException("Cannot convert a successful result to a problem");

			var problem = Results.Problem(statusCode: statusCode);
			var problemDetails = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Extensions =new Dictionary<string, object?>
				{
					{
						"errors",new[]{result.Error}
					}
				}
			};
			return new ObjectResult(problemDetails);
		}
	}
}
